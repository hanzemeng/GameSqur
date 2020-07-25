using UnityEngine;

public class Tool
{
	static int MapSizeX;
	static int MapSizeY;
	static GameObject[][] Map;
	public static void Initialize()
	{
		GridGen StartInfo = GameObject.Find("Initialize").GetComponent<GridGen>();
		MapSizeX = StartInfo.MapRow;
		MapSizeY = StartInfo.MapColumn;
		Map = StartInfo.tempMap;

	}
	public static GameObject GetTile(int XCor, int YCor)
	{
		if (-1 < XCor && -1 < YCor)
		{
			if (XCor < MapSizeX && YCor < MapSizeY)
			{
				return Map[XCor][YCor];
			}
		}
		return null;
	}
	public static GameObject[] GetSurroundTile(GameObject Origin, int Size, string Shape)
    {
		GameObject[] Tile = new GameObject[(2*Size+1)*(2*Size+1)];
		int CenterX = Origin.GetComponent<Tile>().XCor;
		int CenterY = Origin.GetComponent<Tile>().YCor;
		int Count = 0;
		for(int i=0; i<Size*2+1; i++)
		{
			for(int e=0; e<Size*2+1; e++)
			{
				GameObject CurrentTile = GetTile(CenterX-Size+i, CenterY-Size+e);
				if(KeyTerm.RHOMBUS == Shape)
                {
					if(GetDistance(Origin, CurrentTile)<=Size)
                    {
						Tile[Count] = GetTile(CenterX - Size + i, CenterY - Size + e);
						Count++;
					}
                }
				else
                {
					Tile[Count] = GetTile(CenterX-Size+i, CenterY-Size+e);
					Count++;
				}
			}
		}
		return Tile;
    }
	public static float GetDistance(GameObject Start, GameObject Finish)
	{
		if (null == Start || null == Finish)
		{
			return 10000;
		}
		else
		{
			float a, b;
			a = Start.GetComponent<Tile>().XCor - Finish.GetComponent<Tile>().XCor;
			b = Start.GetComponent<Tile>().YCor - Finish.GetComponent<Tile>().YCor;
			return Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2));
		}
	}
	public void DisplayArray(float[] Array)
	{
		for (int i = 0; i < Array.Length; i++)
		{
			Debug.Log(Array[i]);
		}
	}
}
