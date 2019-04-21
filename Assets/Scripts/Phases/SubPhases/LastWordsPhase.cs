using System;

public class LastWordsPhase : TimeBasedSubPhase
{
    private readonly JudgementVoteHandler judgementVotes;

    public LastWordsPhase(
        JudgementVoteHandler judgementVotes,
        float duration)
        : base("Last words", duration)
    {
        this.judgementVotes = judgementVotes;
    }

    protected override void Enter()
    {
    }

    protected override void Exit()
    {
    }

    protected override string GetStateInfo() => null;

    public override bool Enabled()
    {
        return judgementVotes.IsGuilty;
    }
}