public class TrialVoteHandler : VoteHandlerBase<int>
{
    public TrialVoteHandler(GameState gameState)
        : base(gameState)
    {
    }

    public bool CastVoteByPlayerIndex(int requestingPlayerIndex, int targetPlayerIndex)
    {
        return this.CastVote(targetPlayerIndex, requestingPlayerIndex);
    }

    protected override int EmptyResult() => -1;

    protected override void VoteEnded(int votedPlayer, int voteCount, int totalVoteCount)
    {        
        gameState.PutPlayerOnTrialPlayer(votedPlayer, voteCount);
    }
}
