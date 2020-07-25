using UnityEngine;

public class ObjectReference
{
    public static UI UI;
    public static GameObject StartLoc;
    public static GameObject Map;
    public static GameObject Camera;

    public static void Initialize()
    {
        UI = GameObject.Find("UI").GetComponent<UI>();
        StartLoc = GameObject.Find("StartLoc");
        Map = GameObject.Find("Map");
        Camera = GameObject.Find("MainCamera");
    }
}
