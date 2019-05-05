using System.Linq;

public class MafiosoRole : Role
{
    private PlayerController lastAbilityTarget;

    public MafiosoRole()
        : base(
            "Mafioso",
            "Mafia",
            "A member of organized crime, trying to work their way to the top.",
            "Carry out the Godfather's orders.",
            "You can attack if the Godfather doesn't give you orders. " +
            "If the Godfather dies you will become the next Godfather. " +
            "You can talk with the other Mafia at night.")
    {
    }

    public override bool CanUseAbility(PlayerController player, GameState gameState, PlayerController[] targets)
    {
        return !player.Dead && gameState.IsNight && targets != null && targets.Length > 0
               && targets.All(x => x.Role.Alignment != Alignment && x.Dead)
               && targets.All(x => x != player);
    }

    public override void UseAbility(PlayerController player, PlayerController[] targets)
    {
        if (lastAbilityTarget)
        {
            lastAbilityTarget.TargetByMafioso = false;
        }

        targets[0].MarkForKill(player, isMafioso: true);

        lastAbilityTarget = targets[0];
    }
}