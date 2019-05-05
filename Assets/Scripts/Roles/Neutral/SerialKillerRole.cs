using System.Linq;

public class SerialKillerRole : Role
{
    public SerialKillerRole()
        : base(
            "Serial Killer",
            "Neutral",
            "A psychotic criminal who wants everyone to die.",
            "You may choose to attack a player each night.",
            "If you are role blocked you will attack the role blocker instead of your target.",
            false)
    {
    }

    public override bool CanUseAbility(PlayerController player, GameState gameState, PlayerController[] targets)
    {
        return !player.Dead
               && gameState.IsNight
               && targets != null
               && targets.Length > 0
               && targets.All(x => x != player && !x.Dead);
    }

    public override void UseAbility(PlayerController player, PlayerController[] targets)
    {
        targets[0].MarkForKill(player, isSerialKiller: true);
    }
}