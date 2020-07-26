using UnityEngine;

public class ObjectReference
{
    public static GridGen StartInfo;
    public static TextMesh Turn;
    public static GameObject StartLoc;
    public static GameObject Map;
    public static GameObject Camera;
    public static GameObject Move;
    public static GameObject Attack;
    public static GameObject Stop;

    public static void Initialize()
    {
        StartInfo = GameObject.Find("Initialize").GetComponent<GridGen>();
        StartLoc = GameObject.Find("StartLoc");
        Map = GameObject.Find("Map");
        Camera = GameObject.Find("MainCamera");
        Move = GameObject.Find("Move");
        Attack = GameObject.Find("Attack");
        Stop = GameObject.Find("Stop");
        Turn = GameObject.Find("TurnCount").GetComponent<TextMesh>();
    }
}
