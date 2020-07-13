using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static KeyTerm;

public class UI : MonoBehaviour
{
	public string PlayerTeam;
	public int CurrentTurn;
	public GameObject[][] Map;
	public GameObject[] Unit;
	public GameObject[] Target;
	public GameObject Selected;
	public GameObject Move;
	public bool MoveMode;
	public bool Destination;
	public GameObject Attack;
	public bool AttackMode;

	GameObject Camera;
	Vector3 CurrentPosition;
	Vector3 NewPosition;
	bool Check;
	RaycastHit Hit;
	public int MapSizeX;
	public int MapSizeY;
	int CenterX;
	int CenterY;
	int LoadSizeX;
	int LoadSizeY;

	public FindTile[] OpenList;
	public FindTile[] CloseList;
	public int OpenCount;
	public int CloseCount;
	FindTile Route;
	
	void Start()
	{
		Camera = GameObject.Find("MainCamera");
		PlayerTeam = KeyTerm.BLUE;
		CurrentTurn = 1;
		Move = GameObject.Find(KeyTerm.MOVE_CMD);
		Attack = GameObject.Find(KeyTerm.ATTACK_CMD);
		LoadSizeX = 10;
		LoadSizeY = 6;
		Check = false;
		UpdateUnit();
		SortUnit(0);
		OpenSideBar(false);
		DispalyScreen();
		UpdateFog();
	}
	void FixedUpdate()
	{
		if(Input.GetMouseButton(1))
        {
			DragScreen();
        }
		else if(Input.GetMouseButtonUp(1))
        {
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
        	Check = false;
        }
        if(Input.GetKeyDown("e"))
        {
			Route = PathFinding(GetTile(5,5), GetTile(5, 5), GetTile(10, 10));
			AssignRoute(Route);
			Cancel();
        	CurrentTurn++;
        	GameObject.Find("TurnCount").GetComponent<TextMesh>().text = CurrentTurn.ToString();
			UpdateEvent();
        	RemoveUnit();
			UpdateFog();
        }
		if(MoveMode)
		{
			Clear();
			if(Physics.Raycast(Camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition), Vector3.forward, out Hit, 11))
			{
				CenterX = Hit.transform.parent.GetComponent<Tile>().XCor;
				CenterY = Hit.transform.parent.GetComponent<Tile>().YCor;
				Route = PathFinding(Selected.transform.parent.gameObject, Selected.transform.parent.gameObject, GetTile(CenterX, CenterY));
				AssignRoute(Route);
			}
		}
		
	}

	public void AddEvent(string EventType, GameObject Executor, GameObject TargetObject)
    {
		if(KeyTerm.MOVE_CMD == EventType)
		{
			if(Executor.transform.parent.GetComponent<Tile>().XCor == TargetObject.transform.parent.GetComponent<Tile>().XCor && Executor.transform.parent.GetComponent<Tile>().YCor == TargetObject.transform.parent.GetComponent<Tile>().YCor)
			{
				Debug.Log("Same Location");
				return;
			}
			else
			{
				Executor.GetComponent<Unit>().MoveNeed = TargetObject.GetComponent<Tile>().MoveRequire;
				Executor.GetComponent<Unit>().MoveRoute = AssignRoute(Route);
				for (int i = 0; i < Executor.GetComponent<Unit>().MoveRoute.Length; i++)
				{
					if (null == Executor.GetComponent<Unit>().MoveRoute[i])
					{
						Executor.GetComponent<Unit>().Step = i - 2;
						break;
					}
				}
			}
			
		}
		else if(KeyTerm.ATTACK_CMD == EventType)
		{
			if(null == TargetObject.GetComponent<Unit>())
			{
				Debug.Log("Not a Unit");
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
			} 		
		}
	}
	public void ClearUnit(GameObject Unit)
	{
		Unit.GetComponent<SpriteRenderer>().color = Color.white;
		Unit.GetComponent<Unit>().Action = KeyTerm.NONE;
		Unit.GetComponent<Unit>().MoveNeed = 0;
		Unit.GetComponent<Unit>().MoveRoute = new GameObject[Unit.GetComponent<Unit>().MoveRoute.Length];
		Unit.GetComponent<Unit>().Step = 0;
		Target[Unit.GetComponent<Unit>().UnitID] = null;
	}
	void UpdateUnit()
	{
		for(int i=0; i<Unit.Length; i++)
		{
			Unit[i].GetComponent<Unit>().ModifyUnit();
		}
	}
	void SortUnit(int Start)
	{
		for(int i=Start; i<Unit.Length; i++)
		{
			for(int e=i+1; e<Unit.Length; e++)
			{
				if(Unit[i].GetComponent<Unit>().GetProperty(KeyTerm.SPEED) < Unit[e].GetComponent<Unit>().GetProperty(KeyTerm.SPEED))
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
		for(int i=Start; i<Unit.Length; i++)
		{
			Unit[i].GetComponent<Unit>().UnitID = i;
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
				Unit[i].GetComponent<Unit>().Draw(Unit[i].GetComponent<Unit>().LineOfSight, KeyTerm.WARFOG_INDEX, 0,0,0, 0.5f);
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
	
    public void Cancel()
    {
		Clear();
		OpenSideBar(false);
		if(null != Selected)
		{
			Selected.GetComponent<Unit>().Draw(Selected.GetComponent<Unit>().LineOfSight+1, KeyTerm.OUTLINE_INDEX, 1, 1, 1, 0);
		}
		
    	Selected = null;
    	Move.SetActive(false);
    	MoveMode = false;
    	Attack.SetActive(false);
    	AttackMode = false;
    }
	public void OpenSideBar(bool Switch, GameObject Target = null)
    {
		Camera = GameObject.Find("MainCamera");
        GameObject SideBar = GameObject.Find("SideBar");
		GameObject PlaceHolder = GameObject.Find("PlaceHolder");
		GameObject PlaceInfo = GameObject.Find("PlaceInfo");
		if(Switch)
		{
			SideBar.transform.position = new Vector3(Camera.transform.position.x-6,SideBar.transform.position.y,SideBar.transform.position.z);
			int Extra = 0;
			for(int i=0; i<Target.transform.parent.childCount; i++)
			{
				if(Target.transform.parent.GetChild(i).gameObject.name != KeyTerm.OUTLINE && Target.transform.parent.GetChild(i).gameObject.name != KeyTerm.WARFOG)
				{
					GameObject NewItem = Instantiate(PlaceHolder);
					NewItem.transform.parent = SideBar.transform;
					NewItem.transform.position = new Vector3(PlaceHolder.transform.position.x,PlaceHolder.transform.position.y+(i-Extra)*1.5f,-9);
					NewItem.name = Target.transform.parent.GetChild(i).gameObject.name + " C";
					NewItem.GetComponent<SpriteRenderer>().sprite = Target.transform.parent.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite;
					if(!AttackMode && NewItem.name == Target.name + " C")
					{
						NewItem.GetComponent<SpriteRenderer>().color = new Color(1, 1, 0, 1);
					}
					NewItem.AddComponent<BoxCollider2D>();

					NewItem = Instantiate(PlaceInfo);
					NewItem.transform.parent = SideBar.transform;
					NewItem.transform.position = new Vector3(PlaceInfo.transform.position.x,PlaceInfo.transform.position.y+(i-Extra)*1.5f,-9);
					NewItem.name = Target.transform.parent.GetChild(i).gameObject.name + " I";
					NewItem.transform.localScale = new Vector3(0.5f, 0.5f, 1);
					AddText(NewItem);
				}
				else
				{
					Extra++;
				}
			}
			PlaceHolder.SetActive(false);
			PlaceInfo.SetActive(false);
		}
        else
		{
			SideBar.transform.position = new Vector3(Camera.transform.position.x-14,SideBar.transform.position.y,SideBar.transform.position.z);
			for(int i=2; i<SideBar.transform.childCount; i++)
			{
				Destroy(SideBar.transform.GetChild(i).gameObject);
			}
			SideBar.transform.GetChild(0).gameObject.SetActive(true);
			SideBar.transform.GetChild(1).gameObject.SetActive(true);
		}
    }
	void AddText(GameObject Target)
	{
		GameObject ActualObject = GameObject.Find(Target.name.Split(' ')[0]);
		if(null == ActualObject.GetComponent<Tile>())
		{
			Unit UnitInfo = ActualObject.GetComponent<Unit>();
			Target.GetComponent<TextMesh>().text = 
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
			Target.GetComponent<TextMesh>().text = "Tile";
		}
	}

	void UpdateFog()
	{
		for(int i=0; i<Unit.Length; i++)
		{
			if(Unit[i].GetComponent<Unit>().Team == PlayerTeam)
			{
				Unit[i].GetComponent<Unit>().Draw(Unit[i].GetComponent<Unit>().LineOfSight, KeyTerm.WARFOG_INDEX, 0, 0, 0, 0);
			}
			else
			{
				for(int e=0; e<Unit[i].transform.parent.childCount; e++)
				{
					if(KeyTerm.WARFOG == Unit[i].transform.parent.GetChild(e).gameObject.name)
					{
						if(Unit[i].transform.parent.GetChild(e).gameObject.GetComponent<SpriteRenderer>().color == new Color(0,0,0,0))
						{
							Unit[i].SetActive(true);
							//Unit[i].GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
							
						}
						else
						{
							Unit[i].SetActive(false);
							//Unit[i].GetComponent<SpriteRenderer>().color = new Color(1,1,1,0);
						}
					}
				}
			}
		}
	}
	void DragScreen()
    {
		Camera = GameObject.Find("MainCamera");
		
    	if(!Check)
    	{
    		CurrentPosition = Camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
   			Check = true;
 		}
		else
		{
			NewPosition = Camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
			DispalyScreen();
    		Check = false;
		}  
    }
	void DispalyScreen()
	{
		Camera = GameObject.Find("MainCamera");
		
		if(Physics.Raycast(Camera.transform.position, Vector3.forward, out Hit, 11))
		{
			CenterX = Hit.transform.parent.GetComponent<Tile>().XCor;
			CenterY = Hit.transform.parent.GetComponent<Tile>().YCor;
		}
		else
		{
			CenterX = 10;
			CenterY = 6;
			GameObject StartTile = GetTile(CenterX, CenterY);
			Camera.transform.position = new Vector3(StartTile.transform.position.x, StartTile.transform.position.y, -10);
			for(int i=0; i<LoadSizeX*2; i++)
			{
				for(int e=0; e<LoadSizeY*2; e++)
				{
					GameObject Tile =  GetTile(CenterX - i + LoadSizeX, CenterY - e + LoadSizeY);
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
		GameObject NewCenter = GetTile(CenterX, CenterY);
		Camera.transform.position = new Vector3(NewCenter.transform.position.x, NewCenter.transform.position.y, -10);
	}
	void DrawSide(string Side, bool Switch)
	{
		if("Up" == Side)
		{
			for(int i=0; i<LoadSizeX*2; i++)
			{
				GameObject Tile =  GetTile(CenterX - i + LoadSizeX, CenterY - LoadSizeY);
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
				GameObject Tile =  GetTile(CenterX - i + LoadSizeX, CenterY + LoadSizeY);
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
				GameObject Tile =  GetTile(CenterX - LoadSizeX, CenterY + LoadSizeY - i);
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
				GameObject Tile =  GetTile(CenterX + LoadSizeX, CenterY + LoadSizeY - i);
				if(null != Tile)
				{
					Tile.SetActive(Switch);
				}
			}
		}
	}
	public GameObject GetTile(int XCor, int YCor)
	{
		if(-1<XCor && -1<YCor)
		{
			if(XCor<MapSizeX && YCor<MapSizeY)
			{
				return Map[XCor][YCor];
			}
		}
		return null;
	}
	public float GetDistance(GameObject Start, GameObject Finish)
	{
		if(null == Start || null == Finish)
		{
			return 10000;
		}
		else
		{
			float a,b;
			a = Start.GetComponent<Tile>().XCor - Finish.GetComponent<Tile>().XCor;
			b = Start.GetComponent<Tile>().YCor - Finish.GetComponent<Tile>().YCor;
			return Mathf.Sqrt(Mathf.Pow(a,2) + Mathf.Pow(b,2));
		}
	}
	public FindTile PathFinding(GameObject Origin, GameObject Start, GameObject Finish, FindTile Previous = null)
	{
		if(Start == Finish)
        {
			CloseList = new FindTile[CloseList.Length];
			OpenList = new FindTile[OpenList.Length];
			CloseCount = 0;
			OpenCount = 0;

			return Previous;
		}
		if (Previous == null)
        {
			Previous = new FindTile();
			Previous.Previous = new FindTile();
			Previous.Tile = Origin;
			Previous.TileScore = 0;
			CloseList[CloseCount] = Previous;
			CloseCount++;
		}
		else
        {
			CloseList[CloseCount] = OpenList[0];
			CloseCount++;

			for(int i=0; i<OpenCount;i++)
            {
				OpenList[i] = OpenList[i + 1];
            }
			OpenCount--;
		}
		
		int StartX = Start.GetComponent<Tile>().XCor;
		int StartY = Start.GetComponent<Tile>().YCor;
		
		bool Check = false;
		if (Previous.Previous.Tile != GetTile(StartX, StartY-1) && null != GetTile(StartX, StartY-1))
		{
			FindTile Up = new FindTile();
			Up.Previous = Previous;
			Up.Tile = GetTile(StartX, StartY-1);
			Up.TileScore = DistanceEstimate(Up.Tile, Finish) + DistanceEstimate(Up.Tile, Origin) + Previous.TileScore;
			for (int i=0; i< OpenCount; i++)
			{
				if(Up.Tile == OpenList[i].Tile)
				{
					if(OpenList[i].Previous.TileScore > Up.Previous.TileScore)
                    {
						OpenList[i].Previous = Up.Previous;
					}
					Check = true;
					break;
				}
			}
			for (int i = 0; i < CloseCount; i++)
			{
				if (Up.Tile == CloseList[i].Tile)
				{
					Check = true;
					break;
				}
			}
			if (!Check)
			{
				OpenList[OpenCount] = Up;
				OpenCount++;
			}
		}
		Check = false;
		if(Previous.Previous.Tile != GetTile(StartX, StartY+1) && null != GetTile(StartX, StartY+1))
		{
			FindTile Down = new FindTile();
			Down.Previous = Previous;
			Down.Tile = GetTile(StartX, StartY+1);
			Down.TileScore = DistanceEstimate(Down.Tile, Finish) + DistanceEstimate(Down.Tile, Origin) + Previous.TileScore;
			for (int i=0; i< OpenCount; i++)
			{
				if(Down.Tile == OpenList[i].Tile)
				{
					if (OpenList[i].Previous.TileScore > Down.Previous.TileScore)
					{
						OpenList[i].Previous = Down.Previous;
					}
					Check = true;
					break;
				}
			}
			for (int i = 0; i < CloseCount; i++)
			{
				if (Down.Tile == CloseList[i].Tile)
				{
					Check = true;
					break;
				}
			}
			if (!Check)
			{
				OpenList[OpenCount] = Down;
				OpenCount++;
			}
		}
		Check = false;
		if(Previous.Previous.Tile != GetTile(StartX-1, StartY) && null != GetTile(StartX-1, StartY))
		{
			FindTile Left = new FindTile();
			Left.Previous = Previous;
			Left.Tile = GetTile(StartX-1, StartY);
			Left.TileScore = DistanceEstimate(Left.Tile, Finish) + DistanceEstimate(Left.Tile, Origin) + Previous.TileScore;
			for (int i=0; i< OpenCount; i++)
			{
				if(GetTile(StartX-1, StartY) == OpenList[i].Tile)
				{
					if (OpenList[i].Previous.TileScore > Left.Previous.TileScore)
					{
						OpenList[i].Previous = Left.Previous;
					}
					Check = true;
					break;
				}
			}
			for (int i = 0; i < CloseCount; i++)
			{
				if (Left.Tile == CloseList[i].Tile)
				{
					Check = true;
					break;
				}
			}
			if (!Check)
			{
				OpenList[OpenCount] = Left;
				OpenCount++;
			}
		}
		Check = false;
		if(Previous.Previous.Tile != GetTile(StartX+1, StartY) && null != GetTile(StartX+1, StartY))
		{
			FindTile Right = new FindTile();
			Right.Previous = Previous;
			Right.Tile = GetTile(StartX+1, StartY);
			Right.TileScore = DistanceEstimate(Right.Tile, Finish) + DistanceEstimate(Right.Tile, Origin) + Previous.TileScore;
			for(int i=0; i< OpenCount; i++)
			{
				if(GetTile(StartX+1, StartY) == OpenList[i].Tile)
				{
					if(OpenList[i].Previous.TileScore > Right.Previous.TileScore)
					{
						OpenList[i].Previous = Right.Previous;
					}
					Check = true;
					break;
				}
			}
			for (int i = 0; i < CloseCount; i++)
			{
				if (Right.Tile == CloseList[i].Tile)
				{
					Check = true;
					break;
				}
			}
			if (!Check)
			{
				OpenList[OpenCount] = Right;
				OpenCount++;
			}
		}
		for(int i=0; i<OpenCount-1; i++)
        {
			for(int e=i+1; e<OpenCount; e++)
            {
				if(OpenList[i].TileScore> OpenList[e].TileScore)
                {
					FindTile temp = OpenList[e];
					OpenList[e] = OpenList[i];
					OpenList[i] = temp;
				}
			}
		}
		return PathFinding(Origin, OpenList[0].Tile, Finish, OpenList[0]);
	}
	float DistanceEstimate(GameObject Start, GameObject Target)
	{
		float Total = 1;
		if(Start.transform.childCount > KeyTerm.LAND_INDEX && KeyTerm.LAND == Start.transform.GetChild(KeyTerm.LAND_INDEX).gameObject.name)
		{
			Total /= Start.transform.GetChild(KeyTerm.LAND_INDEX).GetComponent<Tile>().SpeedModifier;
		}
		
		if(Start.transform.childCount > KeyTerm.TERRAIN_INDEX && KeyTerm.TERRAIN == Start.transform.GetChild(KeyTerm.TERRAIN_INDEX).gameObject.name)
		{
			Total /= Start.transform.GetChild(KeyTerm.TERRAIN_INDEX).GetComponent<Tile>().SpeedModifier;
		}
		return GetDistance(Start, Target) + Total;
	}
	public GameObject[] AssignRoute(FindTile Route, GameObject[] Path = null, int Count = 0)
    {
		if(null == Path)
        {
			Path = new GameObject[LoadSizeX * LoadSizeY];
        }
		if (null != Route.Previous)
        {
			Path[Count] = Route.Tile;
			Count++;
			Route.Tile.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 0, 1, 1);
			AssignRoute(Route.Previous, Path, Count++);
		}
		return Path;
	}
	public void Clear()
	{
		for(int i=0; i<MapSizeX;i++)
		{
			for(int e=0; e<MapSizeY; e++)
			{
				GetTile(i,e).transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(0,0,0,0);
			}
		}
	}
}

public class FindTile
{
	public GameObject Tile;
	public float TileScore;
	public FindTile Previous;
	public FindTile Next;
}