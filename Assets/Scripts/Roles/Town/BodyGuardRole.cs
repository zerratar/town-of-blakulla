public class BodyGuardRole : Role
{
    public BodyGuardRole()
        : base(
            "Bodyguard",
            "Town",
            "An ex-soldier who secretly makes a living by selling protection.",
            "Protect a player from a direct attack at night.",
            "If your target is attacked or is the victim of a harmful visit, you and the visitor will fight. If you successfully protect someone you can still be Healed.")
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