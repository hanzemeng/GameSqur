using UnityEngine;

public class Attack : MonoBehaviour
{
    void OnMouseDown()
    {
		UI UI = GameObject.Find("UI").GetComponent<UI>();
        UI.ToggleIcon(false);
        UI.OpenSideBar(false);
        UI.PathFind.ClearRoute(UI.Selected.GetComponent<Unit>().MoveRoute);
        UI.Selected.GetComponent<Unit>().Draw(UI.Selected.GetComponent<Unit>().AttackRange, UI.Selected.GetComponent<Unit>().AttackRangeType, KeyTerm.OUTLINE_INDEX, 1, 0, 0, 0.5f);
    	UI.AttackMode = true;
    }
}
