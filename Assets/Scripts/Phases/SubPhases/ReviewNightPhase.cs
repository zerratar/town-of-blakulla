using System;
using UnityEngine;

public class ReviewNightPhase : ConditionBasedSubPhase
{
    private readonly PlayerHandler playerHandler;

    public ReviewNightPhase(PlayerHandler playerHandler)
        : base("Last night")
    {
        this.playerHandler = playerHandler;
    }

    protected override void Enter()
    {
    }

    protected override void Exit()
    {
    }

    public override bool Enabled() => true;

    protected override bool OnCondition(SubPhase phase, GameState state)
    {
        return true;
        //var elapsed = Time.time - this.EnterTime;
        //return elapsed >= 10;
    }

    protected override string GetDebugInfo() => null;
}