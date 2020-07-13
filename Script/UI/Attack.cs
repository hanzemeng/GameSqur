using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static KeyTerm;

public class Attack : MonoBehaviour
{
    void OnMouseDown()
    {
		UI UI = GameObject.Find("UI").GetComponent<UI>();
    	gameObject.SetActive(false);
    	GameObject.Find("Move").SetActive(false);
		UI.OpenSideBar(false, null);
    	UI.Selected.GetComponent<Unit>().Draw(UI.Selected.GetComponent<Unit>().AttackRange, KeyTerm.OUTLINE_INDEX, 1, 0, 0, 0.5f, UI.Selected.GetComponent<Unit>().AttackRangeType);
    	UI.AttackMode = true;
    }
}
