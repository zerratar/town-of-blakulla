using System;

public class DiscussionPhase : TimeBasedSubPhase
{
    public DiscussionPhase(
        float duration)
        : base("Discussion", duration)
    {
    }

    protected override void Enter() { }
    protected override void Exit() { }

    protected override string GetStateInfo() => null;

    public override bool Enabled() => true;
}