using UnityEngine;

public class PlaceHolder : MonoBehaviour
{
    public GameObject ActualUnit;
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
        UI UI = GameObject.Find("UI").GetComponent<UI>();
        if(UI.AttackMode)
        {
            UnitManage.AddEvent("Attack", UI.Selected, ActualUnit);
            UI.Cancel();
            return;
        }
        if(UI.PlayerTeam == ActualUnit.GetComponent<Unit>().Team)
        {
            if(null != UI.Selected)
            {
                PathFinding.ClearRoute(UI.Selected.GetComponent<Unit>().MoveRoute);
            }
            UI.Selected = ActualUnit;
            UI.Selected.GetComponent<Unit>().OnSelecte();
            for (int i = 2; i < gameObject.transform.parent.childCount; i += 2)
            {
                gameObject.transform.parent.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 0, 1);
        }
        else
        {
            Debug.Log(Message.UNIT_NOT_ON_OUR_SIDE);
        }
        
    }
}
