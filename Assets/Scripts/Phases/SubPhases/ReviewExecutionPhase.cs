public class ReviewExecutionPhase : ConditionBasedSubPhase
{
    private readonly GameUI gameUi;
    private readonly PlayerHandler playerHandler;
    private readonly TrialVoteHandler trialVotes;
    private readonly JudgementVoteHandler judgementVote;

    public ReviewExecutionPhase(
        GameUI gameUI,
        PlayerHandler playerHandler,
        TrialVoteHandler trialVotes,
        JudgementVoteHandler judgementVote)
        : base("Execution Review")
    {
        gameUi = gameUI;
        this.playerHandler = playerHandler;
        this.trialVotes = trialVotes;
        this.judgementVote = judgementVote;
    }

    protected override void Enter()
    {
        var player = this.playerHandler.GetPlayerByIndex(this.trialVotes.Result);
        if (!player)
        {
            // oh darn.
            gameUi.ShowMessage("Crap, didn't we have a player that died?", 5f);
            return;
        }

        gameUi.ShowMessage($"<b>{player.PlayerName}</b>, is u ded?", 3f);
        if (!string.IsNullOrEmpty(player.LastWill))
        {
            gameUi.ShowMessage("We found a will next to their body", 2f);
            gameUi.ShowLastWill(player.LastWill, 7f);
        }

        gameUi.ShowMessage($"<b>{player.PlayerName}</b>'s role was <b><color={player.Role.AlignmentColor}>{player.Role.Name}</color></b>", 2.5f);        
    }

    protected override void Exit()
    {
    }

    public override bool Enabled()
    {
        return judgementVote.IsGuilty;
    }

    protected override bool OnCondition(SubPhase phase, GameState state)
    {
        return gameUi.QueuedMessageCount == 0;
    }

    protected override string GetDebugInfo() => null;
}