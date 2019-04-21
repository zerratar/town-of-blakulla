public class JesterRole : Role
{
    public JesterRole() 
        : base("Jester",
            "Neutral",
            "A crazed lunatic whose life goal is to be publicly executed.",
            "Trick the Town into voting against you.",
            "If you are lynched you will attack one of your guilty or abstaining voters the following night with an Unstoppable attack.")
    {
    }

    protected override bool CanUseAbility()
    {
        return false;
    }

    protected override void UseAbility()
    {
    }
}