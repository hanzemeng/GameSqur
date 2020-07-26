using UnityEngine;

public class Hint : MonoBehaviour
{
    void Start()
    {
        SetScreenSize();
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
    void SetScreenSize()
    {
        float Width = Screen.currentResolution.width;
        float Height = Screen.currentResolution.height;
        float X = 16;
        float Y = 9;
        if(Width / Height != X / Y)
        {
            float NewHeight = Width * 9 / 16;
            Screen.SetResolution(Mathf.RoundToInt(Width), Mathf.RoundToInt(NewHeight), false);
        }
        else
        {
            Screen.SetResolution(Mathf.RoundToInt(Width), Mathf.RoundToInt(Height), true);
        }
    }
}