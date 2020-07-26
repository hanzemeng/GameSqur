using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GridGen : MonoBehaviour
{
	public int TotalUnit;
	public GameObject[][] tempMap;
	public int MapRow;
	public int MapColumn;
    void Awake()
    {
		TotalUnit = 0;
		ObjectReference.Initialize();
		DrawSqur(true);

		//MapEdit();
		//SaveMap("/3.SQR");
		InitializeClass();
		
		Destroy(gameObject);
    }
	
    void InitializeClass()
    {
		UnitManage.Initialize();
		Tool.Initialize();
		PathFinding.Initialize();
		UI.Initialize();
	}
	void MapEdit()
    {
		AddUnit("Redelero");
		AddUnit("Redelero");
		AddUnit("Redelero");
		AddUnit("Doppelsoldner");
		AddUnit("Doppelsoldner");
		AddUnit("Doppelsoldner");
		AddUnit("Crossbow");
		AddUnit("Crossbow");
		AddUnit("Crossbow");
		AddUnit("Longbow");
		AddUnit("Longbow");
		AddUnit("Longbow");
		AddUnit("Doppelsoldner", KeyTerm.RED);
		AddUnit("Doppelsoldner", KeyTerm.RED);
		AddUnit("Doppelsoldner", KeyTerm.RED);
		AddUnit("Crossbow", KeyTerm.RED);
		AddUnit("Crossbow", KeyTerm.RED);
		AddUnit("Longbow", KeyTerm.RED);
		AddUnit("Longbow", KeyTerm.RED);
		AddUnit("Longbow", KeyTerm.RED);
		AddUnit("Longbow", KeyTerm.RED);

	}

	void DrawSqur(bool LoadPath = true, int RowCount = 15, int ColumnCount = 15)
    {
		TileInfo Data = null;
		if(LoadPath)
		{
			BinaryFormatter Formatter = new BinaryFormatter();
			string Path = ResourceFile.MapPath + "/Choose.SQR";
			FileStream Stream = new FileStream(Path, FileMode.Open);
			string MapPath = Formatter.Deserialize(Stream) as string;
			Stream.Close();
			Data = LoadMap(MapPath);
			MapRow = Data.MapRow;
			MapColumn = Data.MapColumn;
		}
		else
        {
			MapRow = RowCount;
			MapColumn = ColumnCount;
		}

		tempMap = new GameObject[MapRow][];
		int Count = 0;
		
    	for(int i=0; i< MapRow; i++)
    	{
			GameObject[] tempColum = new GameObject[MapColumn];
    		for(int e=0; e< MapColumn; e++)
    		{
    			GameObject CurrentTile = Instantiate(ObjectReference.StartLoc);
				tempColum[e] = CurrentTile;
    			CurrentTile.transform.parent = ObjectReference.Map.transform;
        		CurrentTile.transform.position = new Vector2(ObjectReference.StartLoc.transform.position.x+i, ObjectReference.StartLoc.transform.position.y-e);
        		CurrentTile.name = i.ToString() + ", " + e.ToString() + " T";
        		CurrentTile.AddComponent<Tile>();
				CurrentTile.GetComponent<Tile>().XCor = i;
				CurrentTile.GetComponent<Tile>().YCor = e;
				if(LoadPath)
                {
					CurrentTile.GetComponent<Tile>().Land = Data.Land[Count];
					CurrentTile.GetComponent<Tile>().Terrain = Data.Terrain[Count];
					CurrentTile.GetComponent<Tile>().Unit = Data.Unit[Count];
					if(null != CurrentTile.GetComponent<Tile>().Unit)
                    {
						CurrentTile.GetComponent<Tile>().UnitID = Data.UnitID[Count];
						CurrentTile.GetComponent<Tile>().UnitTeam = Data.UnitTeam[Count];
						TotalUnit++;
					}
					Count++;
				}
				else
                {
					RandomMap(CurrentTile);
				}
        		
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

		Target.GetComponent<Tile>().Land = Land;
		Target.GetComponent<Tile>().Terrain = Terrain;
	}
	void AddUnit(string Unit, string Team = "Blue", int XCor = -1, int YCor = -1)
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
			CurrentTile.UnitID = TotalUnit;
			CurrentTile.UnitTeam = Team;
			TotalUnit++;
		}
		else
		{
			AddUnit(Unit, Team);
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
	void SaveMap(string Name)
	{
		BinaryFormatter Formatter = new BinaryFormatter();
		string SavePath = ResourceFile.MapPath + Name;
		TileInfo Data = new TileInfo();
		Data.MapRow = MapRow;
		Data.MapColumn = MapColumn;
		Data.Land = new string[MapRow * MapColumn];
		Data.Terrain = new string[MapRow * MapColumn];
		Data.Unit = new string[MapRow * MapColumn];
		Data.UnitID = new int[MapRow * MapColumn];
		Data.UnitTeam = new string[MapRow * MapColumn];
		int Count = 0;
		for(int i = 0; i < MapRow; i++)
		{
			for(int e = 0; e < MapColumn; e++)
			{
				Data.Land[Count] = tempMap[i][e].GetComponent<Tile>().Land;
				Data.Terrain[Count] = tempMap[i][e].GetComponent<Tile>().Terrain;
				Data.Unit[Count] = tempMap[i][e].GetComponent<Tile>().Unit;
				Data.UnitID[Count] = tempMap[i][e].GetComponent<Tile>().UnitID;
				Data.UnitTeam[Count] = tempMap[i][e].GetComponent<Tile>().UnitTeam;
				Count++;
			}
		}
		FileStream Stream = new FileStream(SavePath, FileMode.Create);
		Formatter.Serialize(Stream, Data);
		Stream.Close();
	}
	TileInfo LoadMap(string Path)
	{
		BinaryFormatter Formatter = new BinaryFormatter();
		string LoadPath = ResourceFile.MapPath + Path;
		FileStream Stream = new FileStream(LoadPath, FileMode.Open);
		TileInfo Data = Formatter.Deserialize(Stream) as TileInfo;
		Stream.Close();
		return Data;
	}
}

[System.Serializable]
public class TileInfo
{
	public int MapRow;
	public int MapColumn;
	public string[] Land;
	public string[] Terrain;
	public string[] Unit;
	public int[] UnitID;
	public string[] UnitTeam;
}