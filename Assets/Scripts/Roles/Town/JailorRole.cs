using System.Linq;

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

    public override bool CanUseAbility(PlayerController player, GameState gameState, PlayerController[] targets)
    {
        return !player.Dead
               && gameState.IsDay
               && targets != null
               && targets.Length > 0
               && targets.All(x => x != player);
    }

    public override void UseAbility(PlayerController player, PlayerController[] targets)
    {
        targets[0].Jailed = true;
    }
}