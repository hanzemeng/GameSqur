using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static KeyTerm;

public class GridGen : MonoBehaviour
{
	public int UnitID;
	public GameObject[][] tempMap;
	public int MapRow;
	public int MapColumn;
    void Awake()
    {
    	UnitID = 0;
		MapRow = 30;
		MapColumn = 20;
    	DrawSqur(MapRow, MapColumn);

		AddUnit("Redelero", 6, 2);
		AddUnit("Doppelsoldner", 6, 3, KeyTerm.RED);
		AddUnit("Crossbow", 1, 3);
		AddUnit("Longbow", 5, 5);

		UI UI = GameObject.Find("UI").GetComponent<UI>();
		UI.Map = tempMap;
		UI.MapSizeX = MapRow;
		UI.MapSizeY = MapColumn;
		UI.OpenList = new FindTile[MapRow*MapColumn];
		UI.CloseList = new FindTile[MapRow*MapColumn];
		UI.CloseCount = 0;
		UI.OpenCount = 0;
		UI.Unit = new GameObject[UnitID];
		UI.Target = new GameObject[UnitID];
		Destroy(GameObject.Find("Initialize"));
    }

    void DrawSqur(int RowCount, int ColumnCount)
    {
		tempMap = new GameObject[RowCount][];
		GameObject StartLoc = GameObject.Find("StartLoc");
    	GameObject Map = GameObject.Find("Map");
    	for(int i=0; i<RowCount; i++)
    	{
			GameObject[] tempColum = new GameObject[ColumnCount];
    		for(int e=0; e<ColumnCount; e++)
    		{
    			GameObject CurrentTile = Instantiate(StartLoc);
				tempColum[e] = CurrentTile;
    			CurrentTile.transform.parent = Map.transform;
        		CurrentTile.transform.position = new Vector2(StartLoc.transform.position.x+i,StartLoc.transform.position.y-e);
        		CurrentTile.name = i.ToString() + ", " + e.ToString() + " T";
        		CurrentTile.AddComponent<Tile>();
				CurrentTile.GetComponent<Tile>().XCor = i;
				CurrentTile.GetComponent<Tile>().YCor = e;
        		RandomMap(CurrentTile);
    		}
			tempMap[i] = tempColum;
    	}  
    }
	void RandomMap(GameObject Target)
	{
		string Land = null;
		string Terrain = null;
		
		string[] LandOption = new string[2];
		int[] LandChance = new int[LandOption.Length];
		LandOption[0] = "Grass";
		LandChance[0] = 80;
		LandOption[1] = "Dirt";
		LandChance[1] = 20;
		Land = ChooseOption(LandOption, LandChance);

		string[] TerrainOption = new string[3];
		int[] TerrainChance = new int[TerrainOption.Length];
		TerrainOption[0] = "Tree";
		TerrainChance[0] = 5;
		TerrainOption[1] = "Bush";
		TerrainChance[1] = 10;
		TerrainOption[2] = "Hole";
		TerrainChance[2] = 1;
		Terrain = ChooseOption(TerrainOption, TerrainChance);
		
		
		EditTile(Target.name, Land, Terrain);
	}
    void EditTile(string Target, string Land, string Terrain)
    {
		Tile CurrentTile = GameObject.Find(Target).GetComponent<Tile>();
		CurrentTile.Land = Land;
		CurrentTile.Terrain = Terrain;
    }
	void AddUnit(string Unit, int XCor = -1, int YCor = -1, string Team = "Blue")
	{
		Tile CurrentTile;
		if(-1 == XCor && -1 == YCor)
		{
			CurrentTile = tempMap[Random.Range(0, MapRow)][Random.Range(0, MapColumn)].GetComponent<Tile>();
		}
		else
		{
			CurrentTile = tempMap[XCor][YCor].GetComponent<Tile>();
		}
		if(null == CurrentTile.Unit)
		{
			CurrentTile.Unit = Unit;
			CurrentTile.UnitTeam = Team;
			CurrentTile.ID = UnitID;
			UnitID++;
		}
		else
		{
			AddUnit(Unit);
		}
	}
	string ChooseOption(string[] Type, int[] Prob)
	{
		int Choose = Random.Range(0, 100);
		int temp = 0;
		for(int i=0; i<Type.Length; i++)
		{
			temp += Prob[i];
			if(temp>Choose)
			{
				return Type[i];
			}
		}
		return null;
	}
}