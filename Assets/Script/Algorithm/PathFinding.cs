using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindTile
{
	public GameObject Tile;
	public float TileScore;
	public FindTile Previous;
}

public class PathFinding
{
	static FindTile[] OpenList;
	static FindTile[] CloseList;
	static int OpenCount;
	static int CloseCount;
	public static void Initialize()
    {
		GridGen StartInfo = GameObject.Find("Initialize").GetComponent<GridGen>();
		OpenList = new FindTile[StartInfo.MapRow * StartInfo.MapColumn];
		CloseList = new FindTile[StartInfo.MapRow * StartInfo.MapColumn];
		OpenCount = 0;
		CloseCount = 0;
	}
	public static FindTile FindPath(GameObject Origin, GameObject Start, GameObject Finish, FindTile Previous = null)
	{
		if (Start == Finish)
		{
			Reset();
			return Previous;
		}
		if(Previous == null)
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

			for (int i = 0; i < OpenCount; i++)
			{
				OpenList[i] = OpenList[i + 1];
			}
			OpenCount--;
		}

		int StartX = Start.GetComponent<Tile>().XCor;
		int StartY = Start.GetComponent<Tile>().YCor;

		bool Check = false;
		if(Previous.Previous.Tile != Tool.GetTile(StartX, StartY - 1) && null != Tool.GetTile(StartX, StartY - 1))
		{
			FindTile Up = new FindTile();
			Up.Previous = Previous;
			Up.Tile = Tool.GetTile(StartX, StartY - 1);
			Up.TileScore = DistanceEstimate(Up.Tile, Finish) + DistanceEstimate(Up.Tile, Origin) + Previous.TileScore;
			for (int i = 0; i < OpenCount; i++)
			{
				if (Up.Tile == OpenList[i].Tile)
				{
					if (OpenList[i].Previous.TileScore > Up.Previous.TileScore)
					{
						OpenList[i].Previous = Up.Previous;
						OpenList[i].TileScore = Up.TileScore;
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
		if (Previous.Previous.Tile != Tool.GetTile(StartX, StartY + 1) && null != Tool.GetTile(StartX, StartY + 1))
		{
			FindTile Down = new FindTile();
			Down.Previous = Previous;
			Down.Tile = Tool.GetTile(StartX, StartY + 1);
			Down.TileScore = DistanceEstimate(Down.Tile, Finish) + DistanceEstimate(Down.Tile, Origin) + Previous.TileScore;
			for (int i = 0; i < OpenCount; i++)
			{
				if (Down.Tile == OpenList[i].Tile)
				{
					if (OpenList[i].Previous.TileScore > Down.Previous.TileScore)
					{
						OpenList[i].Previous = Down.Previous;
						OpenList[i].TileScore = Down.TileScore;
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
		if (Previous.Previous.Tile != Tool.GetTile(StartX - 1, StartY) && null != Tool.GetTile(StartX - 1, StartY))
		{
			FindTile Left = new FindTile();
			Left.Previous = Previous;
			Left.Tile = Tool.GetTile(StartX - 1, StartY);
			Left.TileScore = DistanceEstimate(Left.Tile, Finish) + DistanceEstimate(Left.Tile, Origin) + Previous.TileScore;
			for (int i = 0; i < OpenCount; i++)
			{
				if (Tool.GetTile(StartX - 1, StartY) == OpenList[i].Tile)
				{
					if (OpenList[i].Previous.TileScore > Left.Previous.TileScore)
					{
						OpenList[i].Previous = Left.Previous;
						OpenList[i].TileScore = Left.TileScore;
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
		if (Previous.Previous.Tile != Tool.GetTile(StartX + 1, StartY) && null != Tool.GetTile(StartX + 1, StartY))
		{
			FindTile Right = new FindTile();
			Right.Previous = Previous;
			Right.Tile = Tool.GetTile(StartX + 1, StartY);
			Right.TileScore = DistanceEstimate(Right.Tile, Finish) + DistanceEstimate(Right.Tile, Origin) + Previous.TileScore;
			for (int i = 0; i < OpenCount; i++)
			{
				if (Tool.GetTile(StartX + 1, StartY) == OpenList[i].Tile)
				{
					if (OpenList[i].Previous.TileScore > Right.Previous.TileScore)
					{
						OpenList[i].Previous = Right.Previous;
						OpenList[i].TileScore = Right.TileScore;
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
		for (int i = 0; i < OpenCount - 1; i++)
		{
			for (int e = i + 1; e < OpenCount; e++)
			{
				if (OpenList[i].TileScore > OpenList[e].TileScore)
				{
					FindTile temp = OpenList[e];
					OpenList[e] = OpenList[i];
					OpenList[i] = temp;
				}
			}
		}
		return FindPath(Origin, OpenList[0].Tile, Finish, OpenList[0]);
	}
	static float DistanceEstimate(GameObject Start, GameObject Target)
	{
		float Total = 1;
		if (Start.transform.childCount > KeyTerm.LAND_INDEX && KeyTerm.LAND == Start.transform.GetChild(KeyTerm.LAND_INDEX).gameObject.name)
		{
			Total /= Start.transform.GetChild(KeyTerm.LAND_INDEX).GetComponent<Tile>().SpeedModifier;
		}

		if (Start.transform.childCount > KeyTerm.TERRAIN_INDEX && KeyTerm.TERRAIN == Start.transform.GetChild(KeyTerm.TERRAIN_INDEX).gameObject.name)
		{
			Total /= Start.transform.GetChild(KeyTerm.TERRAIN_INDEX).GetComponent<Tile>().SpeedModifier;
		}
		return Tool.GetDistance(Start, Target) + Total;
	}
	public static FindTile FlipRoute(FindTile Route)
	{
		FindTile Current = Route;
		FindTile Result = null;
		while (null != Current)
		{
			FindTile temp = Current;
			Current = Current.Previous;
			temp.Previous = Result;
			Result = temp;
		}
		Result = Result.Previous;
		return Result;
	}
	public static void DrawRoute(FindTile Route)
	{
		if (null != Route && null != Route.Tile)
		{
			Route.Tile.transform.GetChild(KeyTerm.OUTLINE_INDEX).GetComponent<SpriteRenderer>().color = new Color(0, 0, 1, 1);
			DrawRoute(Route.Previous);
		}
	}
	public static void ClearRoute(FindTile Route)
    {
		if (null != Route && null != Route.Tile)
		{
			Route.Tile.transform.GetChild(KeyTerm.OUTLINE_INDEX).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
			ClearRoute(Route.Previous);
		}
	}
	static void Reset()
    {
		CloseList = new FindTile[CloseList.Length];
		OpenList = new FindTile[OpenList.Length];
		CloseCount = 0;
		OpenCount = 0;
	}
}
