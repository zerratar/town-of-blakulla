public class ExecutionPhase : ConditionBasedSubPhase
{
    private readonly WaypointCamera camera;
    private readonly PlayerHandler playerHandler;
    private readonly TrialVoteHandler trialVotes;
    private readonly JudgementVoteHandler judgementVote;

    public ExecutionPhase(
        WaypointCamera camera,
        PlayerHandler playerHandler,
        TrialVoteHandler trialVotes,
        JudgementVoteHandler judgementVote)
        : base("Execution")
    {
        this.camera = camera;
        this.playerHandler = playerHandler;
        this.trialVotes = trialVotes;
        this.judgementVote = judgementVote;
    }

    protected override void Enter()
    {
        this.camera.BeginExecution();
        var player = this.playerHandler.GetPlayerByIndex(this.trialVotes.Result);
        if (player) player.PlayDeathAnimation();
    }

    protected override void Exit()
    {
        this.camera.EndExecution();
        var player = this.playerHandler.GetPlayerByIndex(this.trialVotes.Result);
        if (player) player.Lynched = true;
    }

    public override bool Enabled()
    {
        return judgementVote.IsGuilty;
    }

    protected override string GetStateInfo() => null;

    protected override bool OnCondition(SubPhase phase, GameState state)
    {
        var player = this.playerHandler.GetPlayerByIndex(this.trialVotes.Result);
        return !player || player.IsDeathAnimationOver;
    }
}