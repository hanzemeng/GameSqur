public class Hole : Tile
{
    void Start()
    {
        Initialize();
        TerrainInfo.Use(KeyTerm.HOLE, GetComponent<Tile>());
    }

}
