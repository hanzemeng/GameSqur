using UnityEngine;

public class UnitManage : MonoBehaviour
{
	public static GameObject[] Unit;
	public static GameObject[] Target;

	public static void Initialize()
    {
		Unit = new GameObject[ObjectReference.StartInfo.TotalUnit];
		Target = new GameObject[ObjectReference.StartInfo.TotalUnit];

	}
	public static void AddEvent(string EventType, GameObject Executor, GameObject TargetObject)
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
	public static void UpdateEvent()
	{
		for(int i = 0; i < Unit.Length; i++)
		{
			SortUnit(i);
			if(KeyTerm.NONE != Unit[i].GetComponent<Unit>().Action)
			{
				Unit[i].GetComponent<Unit>().MovePoint += Unit[i].GetComponent<Unit>().GetProperty(KeyTerm.SPEED);
				if(Unit[i].GetComponent<Unit>().MovePoint > Unit[i].GetComponent<Unit>().MoveNeed - 1)
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
	public static void SortUnit(int Start)
	{
		for(int i = Start; i < Unit.Length; i++)
		{
			for(int e = i + 1; e < Unit.Length; e++)
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
		for(int i = Start; i < Unit.Length; i++)
		{
			Unit[i].GetComponent<Unit>().UnitID = i;
		}
	}
	public static void UpdateUnit()
	{
		for(int i = 0; i < Unit.Length; i++)
		{
			Unit[i].GetComponent<Unit>().ModifyUnit();
		}
	}
	static void RemoveUnit()
	{
		GameObject[] tempUnit = Unit;
		GameObject[] tempTarget = Target;
		for(int i = 0; i < Unit.Length; i++)
		{
			if(Unit[i].GetComponent<Unit>().HitPoint <= 0)
			{
				Unit[i].GetComponent<Unit>().Draw(Unit[i].GetComponent<Unit>().LineOfSight, KeyTerm.SQUARE, KeyTerm.WARFOG_INDEX, 0, 0, 0, 0.5f);
				Destroy(Unit[i]);
				Target[i] = null;
				tempUnit = new GameObject[Unit.Length - 1];
				tempTarget = new GameObject[Unit.Length - 1];
				for(int e = 0; e < Unit.Length - 1; e++)
				{
					if(e < i)
					{
						tempUnit[e] = Unit[e];
						tempTarget[e] = Target[e];
					}
					else
					{
						tempUnit[e] = Unit[e + 1];
						tempUnit[e].GetComponent<Unit>().UnitID--;
						tempTarget[e] = Target[e + 1];
					}
				}
				i--;
			}
			Unit = tempUnit;
			Target = tempTarget;
		}
	}
	public static void ClearUnit(GameObject Unit)
	{
		Unit.GetComponent<SpriteRenderer>().color = Color.white;
		Unit.GetComponent<Unit>().Action = KeyTerm.NONE;
		Unit.GetComponent<Unit>().MovePoint = 0;
		Unit.GetComponent<Unit>().MoveNeed = 0;
		PathFinding.ClearRoute(Unit.GetComponent<Unit>().MoveRoute);
		Unit.GetComponent<Unit>().MoveRoute = null;
		Target[Unit.GetComponent<Unit>().UnitID] = null;
	}
}
