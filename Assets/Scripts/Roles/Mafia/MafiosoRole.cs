public class MafiosoRole : Role
{
    public MafiosoRole() 
        : base(
            "Mafioso",
            "Mafia",
            "A member of organized crime, trying to work their way to the top.",
            "Carry out the Godfather's orders.",
            "You can attack if the Godfather doesn't give you orders. If the Godfather dies you will become the next Godfather. You can talk with the other Mafia at night.")
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