public class Redelero : Unit
{
    void Awake()
    {
        InitializeUnit();
    	HitPoint = 250;
        Weapon.Use(KeyTerm.SHORT_SWORD, GetComponent<Unit>());
        Armor.Use(KeyTerm.SCALE_ARMOR, GetComponent<Unit>());
    }
}
