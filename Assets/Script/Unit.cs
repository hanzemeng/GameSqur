using UnityEditor;
using UnityEngine;

public class Unit : MonoBehaviour
{
	public int UnitID;
	public string Team;
	Tile LandModifier;
	Tile TerrainModifier;
	public float MovePoint;
	public float MoveNeed;
	public float HitPoint;
	public float InitialAttack;
	public float AttackNeed;
	public float[] AttackType; //捅，砍，砸，挠，火，水，风，土
	public float InitialSpeed;
	public float InitialDefence;
	public float[] DefenceType; //捅，砍，砸，挠，火，水，风，土
	public int LineOfSight;
	public int AttackRange;
	public string AttackRangeType;
	public string Action;

	public int DetectRange;
	public FindTile MoveRoute;

	public void InitializeUnit()
	{
		AttackType = new float[8];
		DefenceType = new float[8];
		MovePoint = 0;
		HitPoint = 200;
		InitialSpeed = 200;
		LineOfSight = 2;
		Action = KeyTerm.NONE;

		DetectRange = 3;
	}
    public void ModifyUnit()
    {
		if(transform.parent.childCount > KeyTerm.LAND_INDEX && KeyTerm.LAND == transform.parent.GetChild(KeyTerm.LAND_INDEX).gameObject.name)
		{
			LandModifier = transform.parent.GetChild(KeyTerm.LAND_INDEX).gameObject.GetComponent<Tile>();
		}
		else
		{
			LandModifier = GameObject.Find("Map").GetComponent<Tile>();
		}
		if(transform.parent.childCount > KeyTerm.TERRAIN_INDEX && KeyTerm.TERRAIN == transform.parent.GetChild(KeyTerm.TERRAIN_INDEX).gameObject.name)
		{
			TerrainModifier = transform.parent.GetChild(KeyTerm.TERRAIN_INDEX).gameObject.GetComponent<Tile>();
		}
		else
		{
			TerrainModifier = GameObject.Find("Map").GetComponent<Tile>();
		}
    }
	public float GetProperty(string Type, int i=0)
	{
		if(KeyTerm.SPEED == Type)
		{
			return InitialSpeed * LandModifier.SpeedModifier * TerrainModifier.SpeedModifier;
		}
		else if(KeyTerm.ATTACK == Type)
		{
			return InitialAttack * LandModifier.AttackModifier * TerrainModifier.AttackModifier;
		}
		else if(KeyTerm.ATTACK_TYPE == Type)
		{
			return AttackType[i] * LandModifier.AttackTypeModifier[i] * TerrainModifier.AttackTypeModifier[i];
		}
		else if(KeyTerm.DEFENCE == Type)
		{
			return InitialDefence * LandModifier.DefenceModifier * TerrainModifier.DefenceModifier;
		}
		else if(KeyTerm.DEFENCE_TYPE == Type)
		{
			return DefenceType[i] * LandModifier.DefenceTypeModifier[i] * TerrainModifier.DefenceTypeModifier[i];
		}
		else
		{
			return 2.33f;
		}
	}
    public void OnSelecte()
    {
		ObjectReference.UI.ToggleIcon(true, gameObject);
		PathFinding.DrawRoute(MoveRoute);
	}

    public void MoveTo(GameObject Target)
	{
		if(null != MoveRoute.Previous)
        {
			MoveRoute = MoveRoute.Previous;
			Draw(LineOfSight, KeyTerm.SQUARE, KeyTerm.WARFOG_INDEX, 0, 0, 0, 0.5f);
			transform.parent = MoveRoute.Tile.transform;
			transform.position = new Vector3(MoveRoute.Tile.transform.position.x, MoveRoute.Tile.transform.position.y, -2);
		}
		if(transform.parent == Target.transform.parent)
		{
			ObjectReference.UI.ClearUnit(gameObject);
		}
	}
	public void AttackUnit(GameObject Target)
	{
		bool InRange = true;
		if(null != Target)
		{
			if(KeyTerm.SQUARE == AttackRangeType)
			{
				if(Tool.GetDistance(transform.parent.gameObject, Target.transform.parent.gameObject) > AttackRange*Mathf.Sqrt(2))
				{
					InRange = false;
				}
			}
			else if(KeyTerm.RHOMBUS == AttackRangeType)
			{
				if(Tool.GetDistance(transform.parent.gameObject, Target.transform.parent.gameObject) > AttackRange)
				{
					InRange = false;
				}
			}
		}
		else
		{
			Debug.Log(Message.UNIT_DNE);
			InRange = false;
		}
		if(InRange)
		{
			for(int i=0; i<AttackType.Length; i++)
			{
				float Damage = GetProperty(KeyTerm.ATTACK) * GetProperty(KeyTerm.ATTACK_TYPE, i) - Target.GetComponent<Unit>().GetProperty(KeyTerm.DEFENCE) * Target.GetComponent<Unit>().GetProperty(KeyTerm.DEFENCE_TYPE, i);
				if(Damage>0)
				{
					Target.GetComponent<Unit>().HitPoint -= Damage;
				}
			}
			Debug.Log(gameObject.name + " Attacked " + Target.name);
		}
		else
		{
			Debug.Log(Message.OUT_OF_RANGE);
		}
		ObjectReference.UI.ClearUnit(gameObject);
	}

	public void Draw(int Size, string Shape, int Type, float R, float G, float B, float transparency)
	{
		GameObject[] AllTile = Tool.GetSurroundTile(gameObject.transform.parent.gameObject, Size, Shape);
		for(int i=0; i<AllTile.Length; i++)
        {
			if(null != AllTile[i])
            {
				AllTile[i].SetActive(true);
				AllTile[i].transform.GetChild(Type).GetComponent<SpriteRenderer>().color = new Color(R, G, B, transparency);
			}
		}
	}
	void OnMouseEnter()
	{
		Cursor.SetCursor(ResourceFile.HoverCursor, Vector2.zero, CursorMode.ForceSoftware);
	}
	void OnMouseExit()
	{
		Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
	}
	void OnMouseDown()
    {
		if(ObjectReference.UI.AttackMode)
		{
			int temp = 0;
			for(int i=0; i<gameObject.transform.parent.childCount; i++)
			{
				if(null != gameObject.transform.parent.GetChild(i).gameObject.GetComponent<Unit>())
				{
					temp++;
				}
			}
			if(temp>1)
			{
				ObjectReference.UI.OpenSideBar(true, gameObject);
			}
			else
			{
				ObjectReference.UI.AddEvent(KeyTerm.ATTACK_CMD, ObjectReference.UI.Selected, gameObject);
				ObjectReference.UI.Cancel();
			}
		}
		else if(ObjectReference.UI.Destination)
		{
			ObjectReference.UI.Destination = false;
		}
		else
		{
			ObjectReference.UI.OpenSideBar(false);
			ObjectReference.UI.Selected = gameObject;
			ObjectReference.UI.OpenSideBar(true, ObjectReference.UI.Selected);
			ObjectReference.UI.CenterCamera(gameObject.transform.parent.gameObject);
			if (Team == ObjectReference.UI.PlayerTeam)
			{
				OnSelecte();
			}
			else
			{
				Debug.Log(Message.UNIT_NOT_ON_OUR_SIDE);
			}
		}
    }
}
