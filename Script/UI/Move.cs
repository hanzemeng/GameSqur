using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static KeyTerm;
public class Move : MonoBehaviour
{
    void OnMouseDown()
    {
		UI UI = GameObject.Find("UI").GetComponent<UI>();
    	gameObject.SetActive(false);
    	GameObject.Find(KeyTerm.ATTACK_CMD).SetActive(false);
		UI.OpenSideBar(false);
		//UI.Selected.GetComponent<Unit>().Draw(UI.Selected.GetComponent<Unit>().LineOfSight+1, "Outline", 1, 1, 1, 0.5f);
    	UI.MoveMode = true;
    }
}
