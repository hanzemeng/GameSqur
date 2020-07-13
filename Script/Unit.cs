using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static KeyTerm;
using static Weapon;
using static Armor;

public class Unit : MonoBehaviour
{
	UI UI;
	public int UnitID;
	public string Team;
	int UnitXCor;
	int UnitYCor;
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

	public GameObject[] MoveRoute;
	public int Step;

	public void InitializeUnit()
	{
		AttackType = new float[8];
		DefenceType = new float[8];
		MovePoint = 0;
		HitPoint = 200;
		InitialSpeed = 200;
		LineOfSight = 2;
		Action = KeyTerm.NONE;
		Step = 0;
	}
    public void ModifyUnit()
    {
		UI = GameObject.Find("UI").GetComponent<UI>();
		UnitXCor = transform.parent.GetComponent<Tile>().XCor;
		UnitYCor = transform.parent.GetComponent<Tile>().YCor;
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

	public void MoveTo(GameObject Target)
	{
		UI = GameObject.Find("UI").GetComponent<UI>();
		
		GameObject NextStep = MoveRoute[Step];
		Step--;
		Draw(LineOfSight, KeyTerm.WARFOG_INDEX, 0, 0, 0, 0.5f);
    	transform.parent = NextStep.transform;
	    transform.position = new Vector3(NextStep.transform.position.x, NextStep.transform.position.y,-2);
		if(transform.parent == Target.transform.parent)
		{
			UI.ClearUnit(gameObject);
		}
		else
		{
			Action = KeyTerm.MOVE_CMD;
		}
	}
	public void AttackUnit(GameObject Target)
	{
		UI = GameObject.Find("UI").GetComponent<UI>();
		bool InRange = true;
		if(Target.activeSelf)
		{
			if(KeyTerm.SQUARE == AttackRangeType)
			{
				if(UI.GetDistance(transform.parent.gameObject, Target.transform.parent.gameObject) > AttackRange*Mathf.Sqrt(2))
				{
					InRange = false;
				}
			}
			else if(KeyTerm.RHOMBUS == AttackRangeType)
			{
				if(UI.GetDistance(transform.parent.gameObject, Target.transform.parent.gameObject) > AttackRange)
				{
					InRange = false;
				}
			}
		}
		else
		{
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
			Debug.Log(gameObject.name + " Attacked" + " " + Target.name);
		}
		else
		{
			Debug.Log("Out of Range");
		}
		UI.ClearUnit(gameObject);

	}
	public void Draw(int Size, int Type, float R, float G, float B, float transparency, string Shape = "Square")
	{
		UI = GameObject.Find("UI").GetComponent<UI>();
		UnitXCor = transform.parent.GetComponent<Tile>().XCor;
		UnitYCor = transform.parent.GetComponent<Tile>().YCor;
		for(int i=0; i<Size*2+1; i++)
		{
			for(int e=0; e<Size*2+1; e++)
			{
				GameObject CurrentTile = UI.GetTile(UnitXCor-Size+i, UnitYCor-Size+e);
				if(null != CurrentTile)
				{
					if(KeyTerm.RHOMBUS == Shape)
					{
						if(UI.GetDistance(transform.parent.gameObject, CurrentTile) > Size)
						{
							CurrentTile = transform.parent.gameObject;
						}
					}
					CurrentTile.SetActive(true);
					CurrentTile.transform.GetChild(Type).GetComponent<SpriteRenderer>().color = new Color(R, G, B, transparency);
				}			
			}
		}
	}
    void OnMouseDown()
    {
		
		UI = GameObject.Find("UI").GetComponent<UI>();
		if(UI.AttackMode)
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
				UI.OpenSideBar(true, gameObject);
			}
			else
			{
				UI.AddEvent(KeyTerm.ATTACK_CMD, UI.Selected, gameObject);
				UI.Cancel();
			}
		}
		else if(UI.Destination)
		{
			UI.Destination = false;
		}
		else
		{
			UI.OpenSideBar(false);
			UI.Selected = gameObject;
			UI.OpenSideBar(true, UI.Selected);
			if(Team == UI.PlayerTeam)
			{
				UI.Move.SetActive(true);
				UI.Move.transform.position = new Vector3(transform.parent.position.x+1, transform.parent.position.y-1,-3);
				UI.Attack.SetActive(true);
				UI.Attack.transform.position = new Vector3(transform.parent.position.x+1, transform.parent.position.y+1,-3);
			}
			else
			{
				Debug.Log("He/She is not on Your Side");
			}
		}
    }
	
	public void DisplayArray(float[] Array)
	{
		for(int i=0; i<Array.Length; i++)
		{
			Debug.Log(Array[i]);
		}
	}
}
