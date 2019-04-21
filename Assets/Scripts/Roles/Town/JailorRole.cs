public class JailorRole : Role
{
    public JailorRole()
        : base(
            "Jailor",
            "Town",
            "A prison guard who secretly detains suspects.",
            "You may choose one person during the day to jail for the night.",
            "You may anonymously talk with your prisoner. You can choose to attack your prisoner. The jailed target can't perform their night ability. While jailed the prisoner is given Powerful defense.")
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