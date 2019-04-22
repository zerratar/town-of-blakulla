
public class MoveToGallowsPhase : ConditionBasedSubPhase
{
    private readonly GameUI gameUi;
    private readonly TrialVoteHandler trialVotes;
    private readonly PlayerHandler playerHandler;

    public MoveToGallowsPhase(
        GameUI gameUI,
        TrialVoteHandler trialVotes,
        PlayerHandler playerHandler)
        : base("Move to gallows")
    {
        gameUi = gameUI;
        this.trialVotes = trialVotes;
        this.playerHandler = playerHandler;
    }

    protected override void Enter()
    {
        var player = playerHandler.GetPlayerByIndex(trialVotes.Result);
        if (!player)
        {
            UnityEngine.Debug.LogError($"MoveToGallowsPhase: Player with index {trialVotes.Result} not found.");
            return;
        }

        gameUi.ShowMessage($"The town has decided to put <b>{player.PlayerName}</b> on trial.", 
            15f, 
            () => this.HasEnded);
    }

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

    protected override string GetDebugInfo() => null;
}