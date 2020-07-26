using UnityEngine;

public class AI
{
	public static void AIMove()
	{
		for(int i = 0; i < UnitManage.Unit.Length; i++)
		{
			if(UI.PlayerTeam != UnitManage.Unit[i].GetComponent<Unit>().Team)
			{
				GameObject Victim = null;
				if(KeyTerm.NONE == UnitManage.Unit[i].GetComponent<Unit>().Action)
				{
					Victim = CheckEnemy(UnitManage.Unit[i], UnitManage.Unit[i].GetComponent<Unit>().AttackRange, UnitManage.Unit[i].GetComponent<Unit>().AttackRangeType);
					if(null != Victim)
					{
						UnitManage.AddEvent(KeyTerm.ATTACK_CMD, UnitManage.Unit[i], Victim);
					}
					else
					{
						Victim = CheckEnemy(UnitManage.Unit[i], UnitManage.Unit[i].GetComponent<Unit>().DetectRange, KeyTerm.SQUARE);
						if(null != Victim)
						{
							UnitManage.AddEvent(KeyTerm.MOVE_CMD, UnitManage.Unit[i], Victim.transform.parent.GetChild(KeyTerm.LAND_INDEX).gameObject);
						}
					}
				}
				if(KeyTerm.MOVE_CMD == UnitManage.Unit[i].GetComponent<Unit>().Action)
				{
					Victim = CheckEnemy(UnitManage.Unit[i], UnitManage.Unit[i].GetComponent<Unit>().AttackRange, UnitManage.Unit[i].GetComponent<Unit>().AttackRangeType);
					if(null != Victim)
					{
						UnitManage.AddEvent(KeyTerm.ATTACK_CMD, UnitManage.Unit[i], Victim);
					}
				}
				if(KeyTerm.ATTACK_CMD == UnitManage.Unit[i].GetComponent<Unit>().Action)
				{
					Victim = CheckEnemy(UnitManage.Unit[i], UnitManage.Unit[i].GetComponent<Unit>().AttackRange, UnitManage.Unit[i].GetComponent<Unit>().AttackRangeType);
					if(UnitManage.Target[i] != Victim)
					{
						UnitManage.ClearUnit(UnitManage.Unit[i]);
						AIMove();
					}
				}
			}
		}
	}
	static GameObject CheckEnemy(GameObject Unit, int Size, string Shape)
	{
		GameObject[] AllTile = Tool.GetSurroundTile(Unit.transform.parent.gameObject, Size, Shape);
		for(int i = 0; i < AllTile.Length; i++)
		{
			if(null != AllTile[i])
			{
				for(int j = 2; j < AllTile[i].transform.childCount; j++)
				{
					if(null != AllTile[i].transform.GetChild(j).GetComponent<Unit>())
					{
						if(Unit.GetComponent<Unit>().Team != AllTile[i].transform.GetChild(j).GetComponent<Unit>().Team && AllTile[i].transform.GetChild(j).GetComponent<Unit>().HitPoint > 0)
						{
							return AllTile[i].transform.GetChild(j).gameObject;
						}
					}
				}
			}
		}
		return null;
	}
}
