using UnityEngine;

public class Move : MonoBehaviour
{
    void OnMouseDown()
    {
		UI UI = GameObject.Find("UI").GetComponent<UI>();
        UI.ToggleIcon(false);
        UI.OpenSideBar(false);
        UI.PathFind.ClearRoute(UI.Selected.GetComponent<Unit>().MoveRoute);
        UI.MoveMode = true;
    }
}
