public class GodfatherRole : Role
{
    public GodfatherRole()
        : base(
            "Godfather",
            "Mafia",
            "The leader of organized crime.",
            "You may choose to attack a player each night.",
            "If there is a Mafioso he will attack the target instead of you. You will appear to be a Town member to the Sheriff. You can talk with the other Mafia at night.")
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