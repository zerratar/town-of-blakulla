public class ExecutionPhase : ConditionBasedSubPhase
{
    private readonly GameUI gameUi;
    private readonly WaypointCamera camera;
    private readonly PlayerHandler playerHandler;
    private readonly TrialVoteHandler trialVotes;
    private readonly JudgementVoteHandler judgementVote;

    public ExecutionPhase(
        GameUI gameUI,
        WaypointCamera camera,
        PlayerHandler playerHandler,
        TrialVoteHandler trialVotes,
        JudgementVoteHandler judgementVote)
        : base("Execution")
    {
        gameUi = gameUI;
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

        gameUi.ShowMessage($"May God have mercy on your soul, <b>{player.PlayerName}</b>", 30f, () => HasEnded);
    }

    protected override void Exit()
    {
        this.camera.EndExecution();
        var player = this.playerHandler.GetPlayerByIndex(this.trialVotes.Result);
        if (player) player.Dead = true;
    }

    public override bool Enabled()
    {
        return judgementVote.IsGuilty;
    }

    protected override string GetDebugInfo() => null;

    protected override bool OnCondition(SubPhase phase, GameState state)
    {
        var player = this.playerHandler.GetPlayerByIndex(this.trialVotes.Result);
        return !player || player.IsDeathAnimationOver;
    }
}