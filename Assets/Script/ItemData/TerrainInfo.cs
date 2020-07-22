using static KeyTerm;

public class TerrainInfo
{
    //DefenceModifierType: 捅，砍，砸，挠，火，水，风，土
    public static void Use(string Item, Tile Target)
    {
        if(Item == KeyTerm.BUSH)
        {
            Target.SpeedModifier = 0.7f;
            Target.DefenceModifier = 1.2f;
            Target.DefenceTypeModifier[4] = 0.5f;
            Target.DefenceTypeModifier[5] = 1.2f;
            Target.DefenceTypeModifier[6] = 2f;
            Target.DefenceTypeModifier[7] = 1.2f;

        }
        else if(Item == KeyTerm.HOLE)
        {
            Target.SpeedModifier = 0.3f;
            Target.AttackModifier = 0.5f;
            Target.DefenceModifier = 0.9f;
            Target.DefenceTypeModifier[4] = 2f;
            Target.DefenceTypeModifier[5] = 0.1f;
            Target.DefenceTypeModifier[6] = 3f;
            Target.DefenceTypeModifier[7] = 0.2f;
        }
        else if(Item == KeyTerm.TREE)
        {
            Target.SpeedModifier = 0.5f;
            Target.AttackModifier = 1.2f;
            Target.DefenceModifier = 1.5f;
            Target.DefenceTypeModifier[4] = 0.3f;
            Target.DefenceTypeModifier[5] = 1.5f;
            Target.DefenceTypeModifier[6] = 1.5f;
            Target.DefenceTypeModifier[7] = 2f;
        }
    }
}
