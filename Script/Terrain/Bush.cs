public class Bush : Tile
{
    void Start()
    {
        Initialize();
        TerrainInfo.Use(KeyTerm.BUSH, GetComponent<Tile>());
    }
}
