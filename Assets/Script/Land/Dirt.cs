public class Dirt : Tile
{
    void Start()
    {
        Initialize();
	 	LandInfo.Use(KeyTerm.DIRT, GetComponent<Tile>());
    }
}
