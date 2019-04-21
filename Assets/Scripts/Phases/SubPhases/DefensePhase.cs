using System;

public class DefensePhase : TimeBasedSubPhase
{
    private readonly TrialVoteHandler trialVotes;

    public DefensePhase(
        TrialVoteHandler trialVotes,
        float duration)
        : base("Defense", duration)
    {
        this.trialVotes = trialVotes;
    }

    protected override void Enter() { }

    protected override void Exit() { }

    protected override string GetStateInfo() => null;

    public override bool Enabled()
    {
        return trialVotes.Result != -1;
    }
}