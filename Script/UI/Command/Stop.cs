using UnityEngine;

public class Stop : MonoBehaviour
{
    void OnMouseDown()
    {
        UI UI = GameObject.Find("UI").GetComponent<UI>();
        UI.ClearUnit(UI.Selected);
        UI.Cancel();
    }
}
