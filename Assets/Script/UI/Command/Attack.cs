using UnityEngine;

public class Attack : MonoBehaviour
{
    void OnMouseDown()
    {
        UI.ToggleIcon(false);
        UI.OpenSideBar(false);
        PathFinding.ClearRoute(UI.Selected.GetComponent<Unit>().MoveRoute);
        UI.Selected.GetComponent<Unit>().Draw(UI.Selected.GetComponent<Unit>().AttackRange, UI.Selected.GetComponent<Unit>().AttackRangeType, KeyTerm.OUTLINE_INDEX, 1, 0, 0, 0.5f);
    	UI.AttackMode = true;
    }
}
