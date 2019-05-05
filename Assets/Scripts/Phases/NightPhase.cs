using System;
using System.Collections.Generic;

public class NightPhase : Phase
{
    private readonly Action<Phase> onEnter;
    private readonly Action<Phase> onExit;
    private readonly PlayerHandler playerHandler;

    public NightPhase(
        Action<Phase> onEnter,
        Action<Phase> onExit,
        GameUI gameUI,
        TrialVoteHandler trialVotes,
        PlayerHandler playerHandler,
        bool standardMode)
        : base(
            "Night",
            new GoHomePhase(
                gameUI,
                trialVotes,
                playerHandler),
            new DuringNightPhase(standardMode ? 30f : 15f))
    {
        this.onEnter = onEnter;
        this.onExit = onExit;
        this.playerHandler = playerHandler;
    }

    protected override void OnEnter()
    {
        this.playerHandler
            .GetAlivePlayers()
            .ForEach(x => x.ResetAbilityProperties());

        onEnter?.Invoke(this);
    }

    protected override void OnExit() => onExit?.Invoke(this);
}