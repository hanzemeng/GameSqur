using UnityEngine;

public class ResourceFile
{
	public static Texture2D HoverCursor;
    public static Texture2D DragCursor;
	public static string MapPath;

	public static void Initialize()
	{
		HoverCursor = Resources.Load<Texture2D>("Cursor/CursorHand");
		DragCursor = Resources.Load<Texture2D>("Cursor/CursorDrag");
		MapPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/SQUR/Map";
	}
}