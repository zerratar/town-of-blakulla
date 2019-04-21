public class MayorRole : Role
{
    public MayorRole()
        : base(
            "Mayor",
            "Town",
            "The leader of the Town.",
            "You may reveal yourself as the Mayor of the Town.",
            "Once you have revealed yourself as Mayor your vote counts as 3 votes. You may not be healed once you have revealed yourself. Once revealed, you can't whisper or be whispered to.")
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