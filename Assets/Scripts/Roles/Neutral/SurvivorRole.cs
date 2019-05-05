public class SurvivorRole : Role
{
    public SurvivorRole()
        : base(
            "Survivor",
            "Neutral",
            "A neutral character who just wants to live.",
            "Put on a bulletproof vest at night.",
            "Putting on a bulletproof vest gives you Basic defense. You can only use the bulletproof vest 4 times.",
            false)
    {
    }

    public override bool CanUseAbility(PlayerController player, GameState gameState, PlayerController[] targets)
    {
        return !player.Dead && gameState.IsNight && player.ProtectiveVestCounter < 4;
    }

    public override void UseAbility(PlayerController player, PlayerController[] targets)
    {
        player.UseProtectiveVest();
    }
}