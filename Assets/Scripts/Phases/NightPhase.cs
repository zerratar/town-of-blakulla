using System;

public class NightPhase : Phase
{
    private readonly Action<Phase> onEnter;
    private readonly Action<Phase> onExit;

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
            //new DuringNightPhase(standardMode ? 30f : 15f))
            new DuringNightPhase(standardMode ? 30f : 30f))
    {
        this.onEnter = onEnter;
        this.onExit = onExit;
    }

    protected override void OnEnter() => onEnter?.Invoke(this);

    protected override void OnExit() => onExit?.Invoke(this);
}