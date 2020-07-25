using UnityEngine;

public class Hint : MonoBehaviour
{
    void Start()
    {
        ResourceFile.Initialize();
        GetComponent<TextMesh>().text = "Maps store in:\n" + ResourceFile.MapPath;
    }
    void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
/*
public class FolderPath
{
    public static string MapPath = Application.dataPath + "/Map";
}
*/