public class Tree : Tile
{
    void Start()
    {
        Initialize();
        TerrainInfo.Use(KeyTerm.TREE, GetComponent<Tile>());
    }
    
}
