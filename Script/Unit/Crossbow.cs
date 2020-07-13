public class Crossbow : Unit
{
    void Awake()
    {
        InitializeUnit();
    	HitPoint = 150;
        Weapon.Use(KeyTerm.CROSS_BOW, GetComponent<Unit>());
        Armor.Use(KeyTerm.LEATHER_ARMOR, GetComponent<Unit>());
    }
}