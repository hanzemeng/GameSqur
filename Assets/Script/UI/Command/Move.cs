using UnityEngine;

public class Move : MonoBehaviour
{
    void OnMouseDown()
    {
		UI UI = GameObject.Find("UI").GetComponent<UI>();
        UI.ToggleIcon(false);
        UI.OpenSideBar(false);
        PathFinding.ClearRoute(UI.Selected.GetComponent<Unit>().MoveRoute);
        UI.MoveMode = true;
    }
}
