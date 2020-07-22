using static KeyTerm;

public class LandInfo
{
    //DefenceModifierType: 捅，砍，砸，挠，火，水，风，土
    public static void Use(string Item, Tile Target)
    {
        Target.MoveRequire = 500;
        if(Item == KeyTerm.GRASS)
        {
            Target.DefenceModifier = 0.8f;
            Target.DefenceTypeModifier[4] = 0.9f;

        }
        else if(Item == KeyTerm.DIRT)
        {
            Target.SpeedModifier = 0.95f;
            Target.DefenceModifier = 0.8f;
            Target.DefenceTypeModifier[4] = 1.5f;
            Target.DefenceTypeModifier[6] = 0.5f;
        }
    }
}
