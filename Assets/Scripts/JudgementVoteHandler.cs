public class JudgementVoteHandler : VoteHandlerBase<string>
{
    public static string VoteGuilty = "guilty";
    public static string VoteInnocent = "innocent";

    public JudgementVoteHandler(GameState gameState)
        : base(gameState)
    {
    }

    public bool IsGuilty => this.Result == VoteGuilty;

    public bool IsInnocent => this.Result == VoteInnocent || this.Result == null;

    protected override string EmptyResult() => null;

    protected override void VoteEnded(string target, int voteCount, int totalVoteCount)
    {
        gameState.JudgePlayer(target, voteCount);
    }
}