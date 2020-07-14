using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceHolder : MonoBehaviour
{
    void OnMouseDown()
    {
        UI UI = GameObject.Find("UI").GetComponent<UI>();
        GameObject ActualUnit = GameObject.Find(gameObject.name.Split(' ')[0]);
        if(UI.AttackMode)
        {
            UI.AddEvent("Attack", UI.Selected, ActualUnit);
            UI.Cancel();
            return;
        }
        if(UI.PlayerTeam == ActualUnit.GetComponent<Unit>().Team)
        {
            UI.Selected = ActualUnit;
            for (int i = 2; i < gameObject.transform.parent.childCount; i += 2)
            {
                gameObject.transform.parent.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 0, 1);
        }
        else
        {
            Debug.Log("He / She is not on Your Side");
        }
        
    }
}
