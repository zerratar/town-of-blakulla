public class SerialKillerRole : Role
{
    public SerialKillerRole() 
        : base(
            "Serial Killer", 
            "Neutral",
            "A psychotic criminal who wants everyone to die.",
            "You may choose to attack a player each night.",
            "If you are role blocked you will attack the role blocker instead of your target.")
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