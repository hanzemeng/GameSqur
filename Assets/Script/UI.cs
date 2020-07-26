using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
	public static string PlayerTeam;
	public static GameObject Selected;
	public static bool MoveMode;
	public static bool Destination;
	public static bool AttackMode;

	static int CurrentTurn;
	static Vector3 CurrentPosition;
	static Vector3 NewPosition;
	static bool Check;

	static RaycastHit Hit;
	static int CenterX;
	static int CenterY;
	static int LoadSizeX;
	static int LoadSizeY;

	static FindTile Route;
	static int UnitCycle;
	
	public static void Initialize()
	{
		Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
		PlayerTeam = KeyTerm.BLUE;
		CurrentTurn = 1;
		LoadSizeX = 10;
		LoadSizeY = 6;
		Check = false;
		UnitCycle = 0;
	}

    void Start()
    {
		UnitManage.UpdateUnit();
		UnitManage.SortUnit(0);
		OpenSideBar(false);
		DispalyScreen();
		UpdateFog();
		AI.AIMove();
	}
    void Update()
	{
		if(Input.GetKey(KeyCode.Escape))
		{
			SceneManager.LoadScene("LevelSelect");
		}
		else if(Input.GetKeyDown("e"))
		{
			Cancel();
			CurrentTurn++;
			ObjectReference.Turn.text = CurrentTurn.ToString();
			UnitManage.UpdateEvent();
			UpdateFog();
			AI.AIMove();
		}
		else if(Input.GetKeyDown("q"))
		{
			FindUnit();
		}

		if(Input.GetMouseButton(1))
        {
			Cursor.SetCursor(ResourceFile.DragCursor, Vector2.zero, CursorMode.ForceSoftware);
			DragScreen();
			Cancel();
		}
		else if(Input.GetMouseButtonUp(1))
        {
			Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
			Check = false;
        }
		
		if(MoveMode)
		{
			PathFinding.ClearRoute(Route);
			if(Physics.Raycast(ObjectReference.Camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition), Vector3.forward, out Hit, 11))
			{
				int MouseX = Hit.transform.parent.GetComponent<Tile>().XCor;
				int MouseY = Hit.transform.parent.GetComponent<Tile>().YCor;
				Route = PathFinding.FindPath(Selected.transform.parent.gameObject, Selected.transform.parent.gameObject, Tool.GetTile(MouseX, MouseY));
				PathFinding.DrawRoute(Route);
			}
		}
	}

	public static void Cancel()
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
	public static void ToggleIcon(bool Switch, GameObject Unit = null)
	{
		GameObject Icon = GameObject.Find("Icon");
		if(Switch)
        {
			ObjectReference.Move.SetActive(true);
			ObjectReference.Attack.SetActive(true);
			ObjectReference.Stop.SetActive(true);
			Icon.transform.position = new Vector3(Unit.transform.parent.position.x, Unit.transform.parent.position.y, -3);
			Icon.GetComponent<Animator>().SetBool("Play", true);
		}
		else
        {
			ObjectReference.Move.SetActive(false);
			MoveMode = false;
			ObjectReference.Attack.SetActive(false);
			AttackMode = false;
			ObjectReference.Stop.SetActive(false);
			Icon.GetComponent<Animator>().SetBool("Play", false);
		}
	}
	public static void OpenSideBar(bool Switch, GameObject Target = null)
    {
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
	static void AddText(TextMesh Text, GameObject Unit)
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

	static void UpdateFog()
	{
		for(int i=0; i< UnitManage.Unit.Length; i++)
		{
			if(UnitManage.Unit[i].GetComponent<Unit>().Team == PlayerTeam)
			{
				UnitManage.Unit[i].GetComponent<Unit>().Draw(UnitManage.Unit[i].GetComponent<Unit>().LineOfSight, KeyTerm.SQUARE, KeyTerm.WARFOG_INDEX, 0, 0, 0, 0);
			}
		}
		for(int i=0; i< UnitManage.Unit.Length; i++)
		{
			if(UnitManage.Unit[i].GetComponent<Unit>().Team != PlayerTeam)
			{
				if(UnitManage.Unit[i].transform.parent.GetChild(KeyTerm.WARFOG_INDEX).gameObject.GetComponent<SpriteRenderer>().color == new Color(0, 0, 0, 0))
				{
					UnitManage.Unit[i].SetActive(true);
				}
				else
				{
					UnitManage.Unit[i].SetActive(false);
				}
			}
		}
	}
	void FindUnit()
	{
		if(UnitCycle < UnitManage.Unit.Length)
		{
			if(PlayerTeam != UnitManage.Unit[UnitCycle].GetComponent<Unit>().Team)
			{
				UnitCycle++;
				FindUnit();
			}
			else
			{
				CenterCamera(UnitManage.Unit[UnitCycle].transform.parent.gameObject, true);
				OpenSideBar(false);
				OpenSideBar(true, UnitManage.Unit[UnitCycle]);
				UnitCycle++;
			}
		}
		else
		{
			UnitCycle = 0;
			FindUnit();
		}
	}
	public static void CenterCamera(GameObject Target, bool AbsoluteCenter = false)
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
	static void DispalyScreen()
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
	static void DrawSide(string Side, bool Switch)
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