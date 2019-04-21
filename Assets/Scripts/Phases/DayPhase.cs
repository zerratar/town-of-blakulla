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
        WaypointCamera camera,
        PlayerHandler playerHandler,
        TrialVoteHandler trialVoteHandler,
        JudgementVoteHandler judgementVoteHandler,
        bool standardMode)
        : base(
            "Day",
            new GotoTownPhase(playerHandler),

            new DiscussionPhase(
                10f),
            //standardMode ? 45f : 15f)),

            new TrialVotingPhase(
                playerHandler,
                trialVoteHandler,
                30f),
                //30f),

            new MoveToGallowsPhase(trialVoteHandler),

            new DefensePhase(
                trialVoteHandler,
            1f),
                //20f),

            new JudgementPhase(
                    playerHandler,
                    trialVoteHandler,
                    judgementVoteHandler,
                    //20f),
                    20f),

            new LeaveGallowsPhase(
                playerHandler,
                trialVoteHandler,
                judgementVoteHandler),

            new LastWordsPhase(
                judgementVoteHandler,
                1f),
                //5f),

            new ExecutionPhase(
                camera,
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