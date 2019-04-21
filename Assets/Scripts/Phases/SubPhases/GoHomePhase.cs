using System.Linq;
using UnityEngine;

public class GoHomePhase : ConditionBasedSubPhase
{
    private readonly PlayerHandler playerHandler;

    public GoHomePhase(
            PlayerHandler playerHandler)
        : base("Day is over - going home")
    {
        this.playerHandler = playerHandler;
    }

    protected override void Enter()
    {
        playerHandler.GetAlivePlayers().ForEach(x =>
        {
            x.DisableNavigation();
            x.MoveToNextWaypoint();
        });
    }

    protected override void Exit() { }

    public override bool Enabled() => true;

    protected override string GetStateInfo() => null;

    protected override bool OnCondition(SubPhase phase, GameState state)
    {
        var players = playerHandler.GetAlivePlayers();
        return players.All(x => x.HasReached(x.House.GetWaypoint(0)));
    }
}