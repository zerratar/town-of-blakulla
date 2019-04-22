using System;
using System.Linq;

public class GotoTownPhase : ConditionBasedSubPhase
{
    private readonly PlayerHandler playerHandler;

    public GotoTownPhase(
        PlayerHandler playerHandler)
        : base("Move to town")
    {
        this.playerHandler = playerHandler;
    }

    protected override void Enter()
    {
        playerHandler.GetAlivePlayers().ForEach(x =>
        {
            x.ResetPosition();
            x.MoveToNextWaypoint();
        });
    }

    protected override void Exit() { }

    public override bool Enabled() => true;

    protected override string GetDebugInfo() => null;

    protected override bool OnCondition(SubPhase phase, GameState state)
    {
        var alivePlayers = playerHandler.GetAlivePlayers();
        return alivePlayers.All(x => x.HasReached(x.House.GetWaypoint(1)));
    }
}