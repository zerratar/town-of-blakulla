public class MoveToGallowsPhase : ConditionBasedSubPhase
{
    private readonly TrialVoteHandler trialVotes;

    public MoveToGallowsPhase(
        TrialVoteHandler trialVotes)
        : base("Move to gallows")
    {
        this.trialVotes = trialVotes;
    }

    protected override void Enter() { }

    protected override void Exit() { }

    public override bool Enabled()
    {
        return trialVotes.Result != -1;
    }

    protected override bool OnCondition(SubPhase phase, GameState state)
    {
        var playerIndex = trialVotes.Result;
        var player = state.GetPlayerByIndex(playerIndex);
        return player.HasReached(state.GallowPoint.position);
    }

    protected override string GetStateInfo() => null;
}