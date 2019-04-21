using System;

public class LeaveGallowsPhase : ConditionBasedSubPhase
{
    private readonly PlayerHandler playerHandler;
    private readonly TrialVoteHandler trialVotes;
    private readonly JudgementVoteHandler judgementVotes;
    private int trialCounter;

    public LeaveGallowsPhase(
        PlayerHandler playerHandler,
        TrialVoteHandler trialVotes,
        JudgementVoteHandler judgementVotes)
        : base("Player is innocent - leaving gallows")
    {
        this.playerHandler = playerHandler;
        this.trialVotes = trialVotes;
        this.judgementVotes = judgementVotes;
    }

    protected override void Enter()
    {
        var player = playerHandler.GetPlayerByIndex(trialVotes.Result);
        var position = player.House.GetWaypoint(1);
        player.NavigateTo(position);
    }

    protected override void Exit()
    {
        var player = playerHandler.GetPlayerByIndex(
            this.trialVotes.Result);

        if (player) player.DisableNavigation();
        if (++trialCounter >= 3)
            return;

        var votingPhase = GetSubPhaseOfType<TrialVotingPhase>();
        if (votingPhase == null)
            return;

        if (votingPhase.Timer > 0f)
            SetNextSubPhase(votingPhase);
    }

    protected override void AfterReset()
    {
        trialCounter = 0;
    }

    protected override string GetStateInfo() => null;

    protected override bool OnCondition(
        SubPhase phase,
        GameState state)
    {
        var playerIndex = this.trialVotes.Result;
        var player = state.GetPlayerByIndex(playerIndex);
        var outsideGallow = player.House.GetWaypoint(1);
        return player.HasReached(outsideGallow);
    }

    public override bool Enabled()
    {
        return this.trialVotes.HasResult && this.judgementVotes.IsInnocent;
    }
}