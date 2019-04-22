using System;
using UnityEngine;

public class DayPhase : Phase
{
    private readonly Action<Phase> onEnter;
    private readonly Action<Phase> onExit;

    private readonly PlayerHandler playerHandler;
    private readonly TrialVoteHandler trialVoteHandler;
    private readonly JudgementVoteHandler judgementVoteHandler;

    public DayPhase(
        Action<Phase> onEnter,
        Action<Phase> onExit,
        GameUI gameUI,
        WaypointCamera camera,
        PlayerHandler playerHandler,
        TrialVoteHandler trialVoteHandler,
        JudgementVoteHandler judgementVoteHandler,
        bool standardMode)
        : base(
            "Day",

            new GotoTownPhase(playerHandler),

            new ReviewNightPhase(playerHandler),

            new DiscussionPhase(
                5f),
            //standardMode ? 45f : 15f)),

            new TrialVotingPhase(
                gameUI, playerHandler, trialVoteHandler,
                30f),

            new MoveToGallowsPhase(
                gameUI, trialVoteHandler, playerHandler),

            new DefensePhase(
                gameUI,
                playerHandler,
                trialVoteHandler,
            5f),
            //20f),

            new JudgementPhase(
                gameUI,
                playerHandler,
                trialVoteHandler,
                judgementVoteHandler,
                20f),

            new LeaveGallowsPhase(
                gameUI,
                playerHandler,
                trialVoteHandler,
                judgementVoteHandler),

            new LastWordsPhase(
                gameUI,
                judgementVoteHandler,
            5f),

            new ExecutionPhase(
                gameUI,
                camera,
                playerHandler,
                trialVoteHandler,
                judgementVoteHandler),

            new ReviewExecutionPhase(
                gameUI,
                playerHandler,
                trialVoteHandler,
                judgementVoteHandler)
        )
    {
        this.onEnter = onEnter;
        this.onExit = onExit;
        this.playerHandler = playerHandler;
        this.trialVoteHandler = trialVoteHandler;
        this.judgementVoteHandler = judgementVoteHandler;
    }

    protected override void OnEnter()
    {
        this.playerHandler
            .GetDeadPlayers()
            .ForEach(x =>
            {
                x.DisableNavigation();
                x.ResetPosition();
            });

        onEnter?.Invoke(this);
    }

    protected override void OnExit() => onExit?.Invoke(this);
}