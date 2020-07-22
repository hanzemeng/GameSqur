public class Halberdier : Unit
{
    void Awake()
    {
        InitializeUnit();
    	HitPoint = 250;
        AttackNeed = 150;
        LineOfSight = 1;
    }
}
