using System;
using UnityEngine;

public class TrialVotingPhase : ConditionTimeBasedSubPhase
{
    private readonly PlayerHandler playerHandler;
    private readonly TrialVoteHandler voteHandler;

    public TrialVotingPhase(
        PlayerHandler playerHandler,
        TrialVoteHandler voteHandler,
        float duration)
        : base("Voting", duration)
    {
        this.playerHandler = playerHandler;
        this.voteHandler = voteHandler;
    }

    protected override void Enter()
    {
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