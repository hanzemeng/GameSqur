using UnityEngine;

public class Hint : MonoBehaviour
{
    void Start()
    {
        GetComponent<TextMesh>().text = "Drag the CONTENTS of the Map folder into:\n" + FolderPath.MapPath;
    }
    void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
public class FolderPath
{
    public static string MapPath = Application.dataPath + "/Map";
}
