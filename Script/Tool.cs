using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool
{
	public int MapSizeX;
	public int MapSizeY;
	public GameObject[][] Map;
	public GameObject GetTile(int XCor, int YCor)
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
	public float GetDistance(GameObject Start, GameObject Finish)
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
}
