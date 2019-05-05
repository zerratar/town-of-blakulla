using System.Linq;
using UnityEngine;

public class GoHomePhase : ConditionBasedSubPhase
{
    private readonly GameUI gameUi;
    private readonly TrialVoteHandler trialVotes;
    private readonly PlayerHandler playerHandler;

    public GoHomePhase(
            GameUI gameUI,
            TrialVoteHandler trialVotes,
            PlayerHandler playerHandler)
        : base("Day is over - going home")
    {
        gameUi = gameUI;
        this.trialVotes = trialVotes;
        this.playerHandler = playerHandler;
    }

    protected override void Enter()
    {
        if (!trialVotes.HasResult)
        {
            gameUi.ShowMessage("It is too late to continue voting.", 30f, () => this.HasEnded);
        }

        playerHandler.GetAlivePlayers().ForEach(x =>
        {
            x.DisableNavigation();
            x.MoveToNextWaypoint();
        });
    }

    protected override void Exit()
    {
        //this.gameUi.SetMessagePositionDefault();
    }

    public override bool Enabled() => true;

    protected override string GetDebugInfo() => null;

    protected override bool OnCondition(SubPhase phase, GameState state)
    {
        var players = playerHandler.GetAlivePlayers();
        return players.All(x => x.HasReached(x.House.GetWaypoint(0)));
    }
}