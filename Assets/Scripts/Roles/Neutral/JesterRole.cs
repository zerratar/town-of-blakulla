using System.Linq;

public class JesterRole : Role
{
    public JesterRole()
        : base("Jester",
            "Neutral",
            "A crazed lunatic whose life goal is to be publicly executed.",
            "Trick the Town into voting against you.",
            "If you are lynched you will attack one of your guilty or abstaining voters the following night with an Unstoppable attack.")
    {
    }

    public override bool CanUseAbility(PlayerController player, GameState gameState, PlayerController[] targets)
    {
        var canUseAbility = player.Dead && !player.Murderer;
        if (!canUseAbility || targets == null || targets.Length < 1)
        {
            return false;
        }

        var targetablePlayers = gameState.GetAbstainerAndGuiltyVoters();
        return targets.All(x => targetablePlayers.Contains(x));
    }

    public override void UseAbility(PlayerController player, PlayerController[] targets)
    {
        targets[0].Kill(player);
    }
}