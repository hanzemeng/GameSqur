public class Doppelsoldner : Unit
{
    void Awake()
    {
        InitializeUnit();
    	HitPoint = 300;
        Weapon.Use(KeyTerm.LONG_SWORD, GetComponent<Unit>());
        Armor.Use(KeyTerm.PLATE_ARMOR, GetComponent<Unit>());
    }
}
