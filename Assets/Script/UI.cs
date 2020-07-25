using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
	FindTile Route;

	public string PlayerTeam;
	
	public GameObject[] Unit;
	public GameObject[] Target;
	public GameObject Selected;
	public GameObject Move;
	public bool MoveMode;
	public bool Destination;
	public GameObject Attack;
	public bool AttackMode;
	public GameObject Stop;

	int CurrentTurn;
	Vector3 CurrentPosition;
	Vector3 NewPosition;
	bool Check;
	RaycastHit Hit;
	int CenterX;
	int CenterY;
	int LoadSizeX;
	int LoadSizeY;
	int UnitCycle;
	
	void Start()
	{
		Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
		PlayerTeam = KeyTerm.BLUE;
		CurrentTurn = 1;
		Move = GameObject.Find(KeyTerm.MOVE_CMD);
		Attack = GameObject.Find(KeyTerm.ATTACK_CMD);
		Stop = GameObject.Find(KeyTerm.STOP_CMD);
		LoadSizeX = 10;
		LoadSizeY = 6;
		Check = false;
		UnitCycle = 0;
		UpdateUnit();
		SortUnit(0);
		OpenSideBar(false);
		DispalyScreen();
		UpdateFog();
		AI.AIMove();
	}
	void FixedUpdate()
	{
		if(Input.GetKey(KeyCode.Escape))
		{
			SceneManager.LoadScene("LevelSelect");
		}

		if(Input.GetMouseButton(1))
        {
			Cursor.SetCursor(ResourceFile.DragCursor, Vector2.zero, CursorMode.ForceSoftware);
			DragScreen();
        }
		else if(Input.GetMouseButtonUp(1))
        {
			Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
			Check = false;
        }
	}
	void Update()
    {
        if(Input.GetMouseButton(1))
        {
			Cancel();
        }
		else if(Input.GetMouseButtonUp(1))
        {
			Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
			Check = false;
        }
		if(Input.GetKeyDown("e"))
		{
			Cancel();
			CurrentTurn++;
			GameObject.Find("TurnCount").GetComponent<TextMesh>().text = CurrentTurn.ToString();
			UpdateEvent();
			UpdateFog();
			AI.AIMove();
		}
		else if(Input.GetKeyDown("q"))
		{
			FindUnit();
		}
		if(MoveMode)
		{
			PathFinding.ClearRoute(Route);
			if (Physics.Raycast(ObjectReference.Camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition), Vector3.forward, out Hit, 11))
			{
				int MouseX = Hit.transform.parent.GetComponent<Tile>().XCor;
				int MouseY = Hit.transform.parent.GetComponent<Tile>().YCor;
				Route = PathFinding.FindPath(Selected.transform.parent.gameObject, Selected.transform.parent.gameObject, Tool.GetTile(MouseX, MouseY));
				PathFinding.DrawRoute(Route);
			}
		}
	}
    public void AddEvent(string EventType, GameObject Executor, GameObject TargetObject)
    {
		ClearUnit(Executor);
		if(KeyTerm.MOVE_CMD == EventType)
		{
			if(Executor.transform.parent.GetComponent<Tile>().XCor == TargetObject.transform.parent.GetComponent<Tile>().XCor && Executor.transform.parent.GetComponent<Tile>().YCor == TargetObject.transform.parent.GetComponent<Tile>().YCor)
			{
				Debug.Log(Message.SAME_LOCATION);
				return;
			}
			else
			{
				Executor.GetComponent<Unit>().MoveNeed = TargetObject.GetComponent<Tile>().MoveRequire;
				Executor.GetComponent<Unit>().MoveRoute = PathFinding.FlipRoute(PathFinding.FindPath(Executor.transform.parent.gameObject, Executor.transform.parent.gameObject, TargetObject.transform.parent.gameObject));
			}
		}
		else if(KeyTerm.ATTACK_CMD == EventType)
		{
			if(null == TargetObject.GetComponent<Unit>())
			{
				Debug.Log(Message.NOT_A_UNIT);
				return;
			}
			else
			{
				Executor.GetComponent<Unit>().MoveNeed = Executor.GetComponent<Unit>().AttackNeed;
			}
		}
		Target[Executor.GetComponent<Unit>().UnitID] = TargetObject;
		Executor.GetComponent<Unit>().MovePoint = 0;
		Executor.GetComponent<Unit>().Action = EventType;
		Executor.GetComponent<SpriteRenderer>().color = Color.red;
    }
	void UpdateEvent()
	{
		for(int i=0; i<Unit.Length; i++)
    	{
			SortUnit(i);
			if(KeyTerm.NONE != Unit[i].GetComponent<Unit>().Action)
			{
				Unit[i].GetComponent<Unit>().MovePoint += Unit[i].GetComponent<Unit>().GetProperty(KeyTerm.SPEED);
				if(Unit[i].GetComponent<Unit>().MovePoint > Unit[i].GetComponent<Unit>().MoveNeed-1)
    			{
					if(KeyTerm.MOVE_CMD == Unit[i].GetComponent<Unit>().Action)
					{
						Unit[i].GetComponent<Unit>().MoveTo(Target[i]);
					}
					else if(KeyTerm.ATTACK_CMD == Unit[i].GetComponent<Unit>().Action)
					{	
						Unit[i].GetComponent<Unit>().AttackUnit(Target[i]);
					}
					Unit[i].GetComponent<Unit>().MovePoint = 0;
				}
				UpdateUnit(); 
				RemoveUnit();
			} 		
		}
	}
	void SortUnit(int Start)
	{
		for (int i = Start; i < Unit.Length; i++)
		{
			for (int e = i + 1; e < Unit.Length; e++)
			{
				if (Unit[i].GetComponent<Unit>().GetProperty(KeyTerm.SPEED) < Unit[e].GetComponent<Unit>().GetProperty(KeyTerm.SPEED))
				{
					GameObject temp = Unit[i];
					Unit[i] = Unit[e];
					Unit[e] = temp;
					temp = Target[i];
					Target[i] = Target[e];
					Target[e] = temp;
				}
			}
		}
		for (int i = Start; i < Unit.Length; i++)
		{
			Unit[i].GetComponent<Unit>().UnitID = i;
		}
	}
	void UpdateUnit()
	{
		for(int i=0; i<Unit.Length; i++)
		{
			Unit[i].GetComponent<Unit>().ModifyUnit();
		}
	}
    void RemoveUnit()
    {
    	GameObject[] tempUnit = Unit;
    	GameObject[] tempTarget = Target;
    	for(int i=0; i<Unit.Length; i++)
    	{
    		if(Unit[i].GetComponent<Unit>().HitPoint<=0)
    		{
				Unit[i].GetComponent<Unit>().Draw(Unit[i].GetComponent<Unit>().LineOfSight, KeyTerm.SQUARE, KeyTerm.WARFOG_INDEX, 0,0,0, 0.5f);
    			Destroy(Unit[i]);
    			Target[i] = null;
    			tempUnit = new GameObject[Unit.Length - 1];
    			tempTarget = new GameObject[Unit.Length - 1];
    			for(int e=0; e<Unit.Length-1; e++)
    			{
    				if(e<i)
    				{
    					tempUnit[e] = Unit[e];
    					tempTarget[e] = Target[e];
    				}
    				else
    				{
    					tempUnit[e] = Unit[e+1];
    					tempUnit[e].GetComponent<Unit>().UnitID--;
    					tempTarget[e] = Target[e+1];
    				}
    			}
    			i--;
    		}
    		Unit = tempUnit;
    		Target = tempTarget;
    	}
    }
	public void ClearUnit(GameObject Unit)
	{
		Unit.GetComponent<SpriteRenderer>().color = Color.white;
		Unit.GetComponent<Unit>().Action = KeyTerm.NONE;
		Unit.GetComponent<Unit>().MovePoint = 0;
		Unit.GetComponent<Unit>().MoveNeed = 0;
		PathFinding.ClearRoute(Unit.GetComponent<Unit>().MoveRoute);
		Unit.GetComponent<Unit>().MoveRoute = null;
		Target[Unit.GetComponent<Unit>().UnitID] = null;
	}
	void FindUnit()
	{
		if(UnitCycle < Unit.Length)
		{
			if(PlayerTeam != Unit[UnitCycle].GetComponent<Unit>().Team)
			{
				UnitCycle++;
				FindUnit();
			}
			else
			{
				CenterCamera(Unit[UnitCycle].transform.parent.gameObject, true);
				OpenSideBar(false);
				OpenSideBar(true, Unit[UnitCycle]);
				UnitCycle++;
			}
		}
		else
		{
			UnitCycle = 0;
			FindUnit();
		}
	}

	public void Cancel()
    {
		OpenSideBar(false);
		if(null != Selected)
		{
			Selected.GetComponent<Unit>().Draw(Selected.GetComponent<Unit>().LineOfSight+1, KeyTerm.SQUARE, KeyTerm.OUTLINE_INDEX, 1, 1, 1, 0);
			PathFinding.ClearRoute(Selected.GetComponent<Unit>().MoveRoute);
		}
		else
        {
			PathFinding.ClearRoute(Route);
		}
    	Selected = null;
		ToggleIcon(false);
	}
	public void ToggleIcon(bool Switch, GameObject Unit = null)
	{
		GameObject Icon = GameObject.Find("Icon");
		if(Switch)
        {
			Move.SetActive(true);
			Attack.SetActive(true);
			Stop.SetActive(true);
			Icon.transform.position = new Vector3(Unit.transform.parent.position.x, Unit.transform.parent.position.y, -3);
			Icon.GetComponent<Animator>().SetBool("Play", true);
		}
		else
        {
			Move.SetActive(false);
			MoveMode = false;
			Attack.SetActive(false);
			AttackMode = false;
			Stop.SetActive(false);
			Icon.GetComponent<Animator>().SetBool("Play", false);
		}
	}
	public void OpenSideBar(bool Switch, GameObject Target = null)
    {
		//Camera = GameObject.Find("MainCamera");
        GameObject SideBar = GameObject.Find("SideBar");
		GameObject PlaceHolder = SideBar.transform.GetChild(0).gameObject;
		GameObject PlaceInfo = SideBar.transform.GetChild(1).gameObject;
		if (Switch)
		{
			int OffSet = 2;
			for(int i=KeyTerm.LAND_INDEX; i<Target.transform.parent.childCount; i++)
			{
				GameObject Object = Instantiate(PlaceHolder);
				Object.transform.parent = SideBar.transform;
				Object.transform.position = new Vector3(PlaceHolder.transform.position.x,PlaceHolder.transform.position.y+(i-OffSet) *1.5f,-9);
				Object.name = Target.transform.parent.GetChild(i).gameObject.name + " C";
				Object.GetComponent<SpriteRenderer>().sprite = Target.transform.parent.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite;
				Object.AddComponent<BoxCollider2D>();
				if(null != Target.transform.parent.GetChild(i).GetComponent<Unit>())
                {
					Object.GetComponent<PlaceHolder>().ActualUnit = Target.transform.parent.GetChild(i).gameObject;
					if(!AttackMode && PlayerTeam == Target.GetComponent<Unit>().Team && Target == Target.transform.parent.GetChild(i).gameObject)
                    {
						Object.GetComponent<SpriteRenderer>().color = new Color(1, 1, 0, 1);
					}
				}
				Object.SetActive(true);
				GameObject Info = Instantiate(PlaceInfo);
				Info.transform.parent = SideBar.transform;
				Info.transform.position = new Vector3(PlaceInfo.transform.position.x,PlaceInfo.transform.position.y+(i-OffSet) *1.5f,-9);
				Info.name = Target.transform.parent.GetChild(i).gameObject.name + " I";
				Info.transform.localScale = new Vector3(0.5f, 0.5f, 1);
				AddText(Info.GetComponent<TextMesh>(), Object.GetComponent<PlaceHolder>().ActualUnit);
				Info.SetActive(true);
			}
			ObjectReference.Camera.GetComponent<Animator>().SetBool("Open", true);
		}
        else
		{
			ObjectReference.Camera.GetComponent<Animator>().SetBool("Open", false);
			for(int i=2; i<SideBar.transform.childCount; i++)
			{
				Destroy(SideBar.transform.GetChild(i).gameObject);
			}
		}
    }
	void AddText(TextMesh Text, GameObject Unit)
	{
		if(null != Unit)
		{
			Unit UnitInfo = Unit.GetComponent<Unit>();
			Text.text = 
			"HP: " + UnitInfo.HitPoint.ToString("F0") + "  " + 
			"ATK: " + UnitInfo.GetProperty(KeyTerm.ATTACK).ToString("F0") + "  " + 
			"DEF: " + UnitInfo.GetProperty(KeyTerm.DEFENCE).ToString("F0") + "\n" +
			"SPD: " + UnitInfo.GetProperty(KeyTerm.SPEED).ToString("F0") + "  " + 
			"CMD: " + UnitInfo.Action + "\n" +
			"Progress: " + UnitInfo.MovePoint.ToString("F0") + " / " + 
			UnitInfo.MoveNeed.ToString("F0");
		}
		else
		{
			Text.text = "Tile";
		}
	}

	void UpdateFog()
	{
		for(int i=0; i<Unit.Length; i++)
		{
			if(Unit[i].GetComponent<Unit>().Team == PlayerTeam)
			{
				Unit[i].GetComponent<Unit>().Draw(Unit[i].GetComponent<Unit>().LineOfSight, KeyTerm.SQUARE, KeyTerm.WARFOG_INDEX, 0, 0, 0, 0);
			}
		}
		for(int i=0; i<Unit.Length; i++)
		{
			if(Unit[i].GetComponent<Unit>().Team != PlayerTeam)
			{
				if (Unit[i].transform.parent.GetChild(KeyTerm.WARFOG_INDEX).gameObject.GetComponent<SpriteRenderer>().color == new Color(0, 0, 0, 0))
				{
					Unit[i].SetActive(true);
				}
				else
				{
					Unit[i].SetActive(false);
				}
			}
		}
	}
	public void CenterCamera(GameObject Target, bool AbsoluteCenter = false)
    {
		int TargetX = Target.GetComponent<Tile>().XCor;
		int TargetY = Target.GetComponent<Tile>().YCor;
		for(int i = 0; i < LoadSizeX * 2; i++)
		{
			for(int e = 0; e < LoadSizeY * 2; e++)
			{
				GameObject Tile = Tool.GetTile(CenterX - i + LoadSizeX, CenterY - e + LoadSizeY);
				if(null != Tile)
				{
					Tile.SetActive(false);
				}
			}
		}
		if(AbsoluteCenter)
        {
			ObjectReference.Camera.transform.position = new Vector3(Target.transform.position.x , Target.transform.position.y, -10);
			CenterX = TargetX;
			CenterY = TargetY;
		}
		else
        {
			if(CenterX - TargetX > 1)
			{
				ObjectReference.Camera.transform.position = new Vector3(Target.transform.position.x + 1, ObjectReference.Camera.transform.position.y, -10);
				CenterX = TargetX + 1;
			}
			else if(TargetX - CenterX > 7)
			{
				ObjectReference.Camera.transform.position = new Vector3(Target.transform.position.x - 7, ObjectReference.Camera.transform.position.y, -10);
				CenterX = TargetX - 7;
			}
			if(TargetY - CenterY < -3)
			{
				ObjectReference.Camera.transform.position = new Vector3(ObjectReference.Camera.transform.position.x, Target.transform.position.y - 3, -10);
				CenterY = TargetY + 3;
			}
			else if(CenterY - TargetY < -3)
			{
				ObjectReference.Camera.transform.position = new Vector3(ObjectReference.Camera.transform.position.x, Target.transform.position.y + 3, -10);
				CenterY = TargetY - 3;
			}
		}
		for(int i = 0; i < LoadSizeX * 2; i++)
		{
			for(int e = 0; e < LoadSizeY * 2; e++)
			{
				GameObject Tile = Tool.GetTile(CenterX - i + LoadSizeX, CenterY - e + LoadSizeY);
				if(null != Tile)
				{
					Tile.SetActive(true);
				}
			}
		}
	}
	void DragScreen()
    {
    	if(!Check)
    	{
    		CurrentPosition = ObjectReference.Camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
   			Check = true;
 		}
		else
		{
			NewPosition = ObjectReference.Camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
			DispalyScreen();
    		Check = false;
		}  
    }
	void DispalyScreen()
	{
		if(Physics.Raycast(ObjectReference.Camera.transform.position, Vector3.forward, out Hit, 11))
		{
			CenterX = Hit.transform.parent.GetComponent<Tile>().XCor;
			CenterY = Hit.transform.parent.GetComponent<Tile>().YCor;
		}
		else
		{
			CenterX = 12;
			CenterY = 6;
			GameObject StartTile = Tool.GetTile(CenterX, CenterY);
			ObjectReference.Camera.transform.position = new Vector3(StartTile.transform.position.x, StartTile.transform.position.y, -10);
			for(int i=0; i<LoadSizeX*2; i++)
			{
				for(int e=0; e<LoadSizeY*2; e++)
				{
					GameObject Tile = Tool.GetTile(CenterX - i + LoadSizeX, CenterY - e + LoadSizeY);
					if(null != Tile)
					{
						Tile.SetActive(true);
					}
				}
			}
			return;
		}
		if(CurrentPosition.x - NewPosition.x>0.05)
		{
			DrawSide("Right", true);
			DrawSide("Left", false);
			CenterX++;
		}
		else if(NewPosition.x - CurrentPosition.x>0.05)
		{
			DrawSide("Left", true);
			DrawSide("Right", false);
			CenterX--;
		}
		if(CurrentPosition.y - NewPosition.y>0.05)
		{
			DrawSide("Up", true);
			DrawSide("Down", false);
			CenterY--;
		}
		else if(NewPosition.y - CurrentPosition.y>0.05)
		{
			DrawSide("Down", true);
			DrawSide("Up", false);
			CenterY++;
		}
		GameObject NewCenter = Tool.GetTile(CenterX, CenterY);
		ObjectReference.Camera.transform.position = new Vector3(NewCenter.transform.position.x, NewCenter.transform.position.y, -10);
	}
	void DrawSide(string Side, bool Switch)
	{
		if("Up" == Side)
		{
			for(int i=0; i<LoadSizeX*2; i++)
			{
				GameObject Tile = Tool.GetTile(CenterX - i + LoadSizeX, CenterY - LoadSizeY);
				if(null != Tile)
				{
					Tile.SetActive(Switch);
				}
			}
		}
		else if("Down" == Side)
		{
			for(int i=0; i<LoadSizeX*2; i++)
			{
				GameObject Tile = Tool.GetTile(CenterX - i + LoadSizeX, CenterY + LoadSizeY);
				if(null != Tile)
				{
					Tile.SetActive(Switch);
				}
			}
		}
		else if("Left" == Side)
		{
			for(int i=0; i<LoadSizeY*2; i++)
			{
				GameObject Tile = Tool.GetTile(CenterX - LoadSizeX, CenterY + LoadSizeY - i);
				if(null != Tile)
				{
					Tile.SetActive(Switch);
				}
			}
		}
		else if("Right" == Side)
		{
			for(int i=0; i<LoadSizeY*2; i++)
			{
				GameObject Tile = Tool.GetTile(CenterX + LoadSizeX, CenterY + LoadSizeY - i);
				if(null != Tile)
				{
					Tile.SetActive(Switch);
				}
			}
		}
	}
}