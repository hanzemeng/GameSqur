using static KeyTerm;

public class Weapon
{
    //AttackType: 捅，砍，砸，挠，火，水，风，土

    public static void Use(string Item, Unit Target)
    {
        if(Item == KeyTerm.SHORT_SWORD)
        {
            Target.InitialAttack = 75;
            Target.AttackType[1] = 0.9f;
            Target.AttackType[2] = 0.1f;
            Target.AttackNeed = 150;
            Target.AttackRange = 1;
            Target.AttackRangeType = KeyTerm.RHOMBUS;
            Target.InitialSpeed -= 10;
        }
        else if(Item == KeyTerm.LONG_SWORD)
        {
            Target.InitialAttack = 200;
            Target.AttackType[1] = 0.8f;
            Target.AttackType[2] = 0.2f;
            Target.AttackNeed = 300;
            Target.AttackRange = 1;
            Target.AttackRangeType = KeyTerm.SQUARE;
            Target.InitialSpeed -= 50;
        }
        else if(Item == KeyTerm.CROSS_BOW)
        {
            Target.InitialAttack = 75;
            Target.AttackType[0] = 0.95f;
            Target.AttackType[2] = 0.05f;
            Target.AttackNeed = 200;
            Target.AttackRange = 2;
            Target.AttackRangeType = KeyTerm.RHOMBUS;
            Target.InitialSpeed -= 20;
        }
        else if(Item == KeyTerm.LONG_BOW)
        {
            Target.InitialAttack = 100;
            Target.AttackType[0] = 0.95f;
            Target.AttackType[2] = 0.05f;
            Target.AttackNeed = 250;
            Target.AttackRange = 2;
            Target.AttackRangeType = KeyTerm.SQUARE;
            Target.InitialSpeed -= 40;
        }
    }
    
}
