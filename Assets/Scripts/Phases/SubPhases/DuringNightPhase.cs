using System;

public class DuringNightPhase : TimeBasedSubPhase
{
    public DuringNightPhase(
        float duration)
        : base("Night", duration)
    {
    }

    protected override void Enter() { }

    protected override void Exit() { }

    protected override string GetStateInfo() => null;

    public override bool Enabled() => true;
}