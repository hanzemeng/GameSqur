﻿using UnityEngine;

public class Tile : MonoBehaviour
{
	public int XCor;
	public int YCor;
	public string Land;
    public string Terrain;
    public string Unit;
	public int UnitID;
	public string UnitTeam;
	public float MoveRequire;
    public float SpeedModifier;
	public float AttackModifier;
	public float[] AttackTypeModifier;
	public float DefenceModifier;
	public float[] DefenceTypeModifier;
	void Start()
	{
		DrawMap();
	}
	public void Initialize()
	{
		SpeedModifier = 1;
        AttackModifier = 1;
        AttackTypeModifier = new float[8];
        for(int i=0; i<AttackTypeModifier.Length;i++)
        {
            AttackTypeModifier[i] = 1f;
        } 
        DefenceModifier = 1;
        DefenceTypeModifier = new float[8];
        for(int i=0; i<DefenceTypeModifier.Length;i++)
        {
            DefenceTypeModifier[i] = 1f;
        }
	}
    void DrawMap()
    {
		UI UI = GameObject.Find("UI").GetComponent<UI>();
		GameObject temp;

		temp = Instantiate(GameObject.Find(KeyTerm.OUTLINE));
		temp.transform.parent = transform;
		temp.transform.name = KeyTerm.OUTLINE;
		temp.transform.position = new Vector3(transform.position.x, transform.position.y,-3);
		
		temp = Instantiate(GameObject.Find(KeyTerm.WARFOG));
		temp.transform.parent = transform;
		temp.transform.name = KeyTerm.WARFOG;
		temp.transform.position = new Vector3(transform.position.x, transform.position.y,-4);

		temp = Instantiate(GameObject.Find(Land));
		temp.transform.parent = transform;
		temp.transform.name = KeyTerm.LAND;
		temp.transform.position = new Vector3(transform.position.x, transform.position.y,0);
		
		if(null != Terrain)
    	{
    		temp = Instantiate(GameObject.Find(Terrain));
    		temp.transform.parent = transform;
    		temp.transform.name = KeyTerm.TERRAIN;
    		temp.transform.position = new Vector3(transform.position.x, transform.position.y,-1);
    	}

		if(null != Unit)
    	{
    		temp = Instantiate(GameObject.Find(Unit));
    		temp.name = GameObject.Find(Unit).name;
			UnitManage.Unit[UnitID] = temp;
			temp.GetComponent<Unit>().Team = UnitTeam;
    		temp.transform.parent = transform;
    		temp.transform.position = new Vector3(transform.position.x, transform.position.y,-2);
    	}
		
    	gameObject.SetActive(false);
    }
    void OnMouseDown()
    {
    	UI UI = GameObject.Find("UI").GetComponent<UI>();

    	if(UI.MoveMode)
    	{
			UnitManage.AddEvent(KeyTerm.MOVE_CMD, UI.Selected, gameObject);
			for(int i=0; i<gameObject.transform.parent.childCount; i++)
			{
				if(null != gameObject.transform.parent.GetChild(i).gameObject.GetComponent<Unit>())
				{
					UI.Destination = true;
					break;
				}
			}
			UI.Cancel();
    	}
    }
}