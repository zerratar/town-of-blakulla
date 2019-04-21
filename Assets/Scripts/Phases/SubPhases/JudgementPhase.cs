using System;

public class JudgementPhase : ConditionTimeBasedSubPhase
{
    private readonly PlayerHandler playerHandler;
    private readonly TrialVoteHandler trialVotes;
    private readonly JudgementVoteHandler judgementVotes;

    public JudgementPhase(
        PlayerHandler playerHandler,
        TrialVoteHandler trialVotes,
        JudgementVoteHandler judgementVotes,
        float duration)
        : base("Judgement", duration)
    {
        this.playerHandler = playerHandler;
        this.trialVotes = trialVotes;
        this.judgementVotes = judgementVotes;
    }

    protected override void Enter()
    {
        judgementVotes.BeginVoting();
    }

    protected override void Exit()
    {
        judgementVotes.EndVoting();
    }

    public override bool Enabled()
    {
        return trialVotes.Result != -1;
    }

    protected override bool OnCondition(SubPhase phase, GameState state)
    {
        var requiredVotes = judgementVotes.GetRequiredVoteCount();
        var voteCount = judgementVotes.VoteCount;
        var result = voteCount > requiredVotes;
        return result;
    }

    protected override string GetStateInfo()
    {
        return $"{judgementVotes.VoteCount}/{playerHandler.GetAssignedPlayers().Count} voted.";
    }
}