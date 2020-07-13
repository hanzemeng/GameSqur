public class Longbow : Unit
{
    void Awake()
    {
        InitializeUnit();
    	HitPoint = 150;
        Weapon.Use(KeyTerm.LONG_BOW, GetComponent<Unit>());
        Armor.Use(KeyTerm.LEATHER_ARMOR, GetComponent<Unit>());
    }
}
