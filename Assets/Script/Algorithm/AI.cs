using UnityEngine;

public class AI
{
	public UI UI;
	public void AIMove()
	{
		for(int i = 0; i < UI.Unit.Length; i++)
		{
			if(UI.PlayerTeam != UI.Unit[i].GetComponent<Unit>().Team)
			{
				GameObject Victim = null;
				if(KeyTerm.NONE == UI.Unit[i].GetComponent<Unit>().Action)
				{
					Victim = CheckEnemy(UI.Unit[i], UI.Unit[i].GetComponent<Unit>().AttackRange, UI.Unit[i].GetComponent<Unit>().AttackRangeType);
					if(null != Victim)
					{
						UI.AddEvent(KeyTerm.ATTACK_CMD, UI.Unit[i], Victim);
					}
					else
					{
						Victim = CheckEnemy(UI.Unit[i], UI.Unit[i].GetComponent<Unit>().DetectRange, KeyTerm.SQUARE);
						if(null != Victim)
						{
							UI.AddEvent(KeyTerm.MOVE_CMD, UI.Unit[i], Victim.transform.parent.GetChild(KeyTerm.LAND_INDEX).gameObject);
						}
					}
				}
				if(KeyTerm.MOVE_CMD == UI.Unit[i].GetComponent<Unit>().Action)
				{
					Victim = CheckEnemy(UI.Unit[i], UI.Unit[i].GetComponent<Unit>().AttackRange, UI.Unit[i].GetComponent<Unit>().AttackRangeType);
					if(null != Victim)
					{
						UI.AddEvent(KeyTerm.ATTACK_CMD, UI.Unit[i], Victim);
					}
				}
				if(KeyTerm.ATTACK_CMD == UI.Unit[i].GetComponent<Unit>().Action)
				{
					Victim = CheckEnemy(UI.Unit[i], UI.Unit[i].GetComponent<Unit>().AttackRange, UI.Unit[i].GetComponent<Unit>().AttackRangeType);
					if(UI.Target[i] != Victim)
					{
						UI.ClearUnit(UI.Unit[i]);
						AIMove();
					}
				}
			}
		}
	}
	GameObject CheckEnemy(GameObject Unit, int Size, string Shape)
	{
		GameObject[] AllTile = UI.Tool.GetSurroundTile(Unit.transform.parent.gameObject, Size, Shape);
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
