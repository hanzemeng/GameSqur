using static KeyTerm;

public class Armor
{
    //DefenceType: 捅，砍，砸，挠，火，水，风，土
    public static void Use(string Item, Unit Target)
    {
        if(Item == KeyTerm.SCALE_ARMOR)
        {
            Target.InitialDefence = 30;
            Target.DefenceType[0] = 1;
            Target.DefenceType[1] = 0.5f;
            Target.DefenceType[2] = 0.8f;
            Target.DefenceType[3] = 0.2f;
            Target.InitialSpeed -= 30;
        }
        else if(Item == KeyTerm.PLATE_ARMOR)
        {
            Target.InitialDefence = 50;
            Target.DefenceType[0] = 1;
            Target.DefenceType[1] = 1;
            Target.DefenceType[2] = -0.2f;
            Target.DefenceType[3] = 1;
            Target.InitialSpeed -= 70;
        }
        else if(Item == KeyTerm.LEATHER_ARMOR)
        {
            Target.InitialDefence = 10;
            Target.DefenceType[0] = 1;
            Target.DefenceType[1] = 1;
            Target.DefenceType[2] = 1;
            Target.DefenceType[3] = 1;
            Target.InitialSpeed -= 5;
        }
    }
    
}
