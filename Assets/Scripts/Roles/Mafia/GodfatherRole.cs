using System.Linq;

public class GodfatherRole : Role
{
    private PlayerController lastAbilityTarget;

    public GodfatherRole()
        : base(
            "Godfather",
            "Mafia",
            "The leader of organized crime.",
            "You may choose to attack a player each night.",
            "If there is a Mafioso he will attack the target instead of you. You will appear to be a Town member to the Sheriff. You can talk with the other Mafia at night.")
    {
    }

    public override bool CanUseAbility(PlayerController player, GameState gameState, PlayerController[] targets)
    {
        return !player.Dead && gameState.IsNight && targets != null && targets.Length > 0
               && targets.All(x => x != player && x.Role.Alignment != Alignment && x.Dead);
    }

    public override void UseAbility(PlayerController player, PlayerController[] targets)
    {
        if (lastAbilityTarget)
        {
            lastAbilityTarget.TargetByGodfather = false;
        }

        targets[0].MarkForKill(player, true);

        lastAbilityTarget = targets[0];
    }
}