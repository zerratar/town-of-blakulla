public class SurvivorRole : Role
{
    public SurvivorRole() 
        : base(
            "Survivor",
            "Neutral",
            "A neutral character who just wants to live.",
            "Put on a bulletproof vest at night.",
            "Putting on a bulletproof vest gives you Basic defense. You can only use the bulletproof vest 4 times.")
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