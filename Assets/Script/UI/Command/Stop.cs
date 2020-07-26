using UnityEngine;

public class Stop : MonoBehaviour
{
    void OnMouseDown()
    {
        UnitManage.ClearUnit(UI.Selected);
        UI.Cancel();
    }
}
