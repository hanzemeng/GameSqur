public class Grass : Tile
{
    void Start()
    {
        Initialize();
	 	LandInfo.Use(KeyTerm.GRASS, GetComponent<Tile>());
    }
}
