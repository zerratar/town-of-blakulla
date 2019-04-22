using System;
using UnityEngine;

public class TrialVotingPhase : ConditionTimeBasedSubPhase
{
    private readonly GameUI gameUi;
    private readonly PlayerHandler playerHandler;
    private readonly TrialVoteHandler voteHandler;

    public TrialVotingPhase(
        GameUI gameUI,
        PlayerHandler playerHandler,
        TrialVoteHandler voteHandler,
        float duration)
        : base("Voting", duration)
    {
        gameUi = gameUI;
        this.playerHandler = playerHandler;
        this.voteHandler = voteHandler;
    }

    protected override void Enter()
    {
        gameUi.ShowMessage("Todays public vote will now begin.", 2.5f);
        voteHandler.BeginVoting();
    }

    protected override void Exit()
    {
        voteHandler.EndVoting();
    }

    public override bool Enabled() => true;

    protected override bool OnCondition(SubPhase phase, GameState state)
    {
        var requiredVotes = voteHandler.GetRequiredVoteCount();
        var voteCount = voteHandler.VoteCount;
        var result = voteCount > requiredVotes;
        return result;
    }

    protected override string GetStateInfo()
    {
        return $"{voteHandler.VoteCount}/{playerHandler.GetAssignedPlayers().Count} voted.";
    }
}