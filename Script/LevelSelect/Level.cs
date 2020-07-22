using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Level : MonoBehaviour
{
    public string LevelPath;

    void OnMouseEnter()
    {
        Cursor.SetCursor(CursorImage.HoverCursor, Vector2.zero, CursorMode.ForceSoftware);
    }
    void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
    }
    void OnMouseDown()
    {
        BinaryFormatter Formatter = new BinaryFormatter();
        string SavePath = FolderPath.MapPath + "/Choose.SQR";
        FileStream Stream = new FileStream(SavePath, FileMode.Create);
        Formatter.Serialize(Stream, LevelPath);
        Stream.Close();
        SceneManager.LoadScene("Game");
    }
}
