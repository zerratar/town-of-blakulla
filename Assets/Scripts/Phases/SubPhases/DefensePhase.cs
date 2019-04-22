using System;

public class DefensePhase : TimeBasedSubPhase
{
    private readonly GameUI gameUi;
    private readonly PlayerHandler playerHandler;
    private readonly TrialVoteHandler trialVotes;

    public DefensePhase(
        GameUI gameUI,
        PlayerHandler playerHandler,
        TrialVoteHandler trialVotes,
        float duration)
        : base("Defense", duration)
    {
        gameUi = gameUI;
        this.playerHandler = playerHandler;
        this.trialVotes = trialVotes;
    }

    protected override void Enter()
    {
        var player = playerHandler.GetPlayerByIndex(trialVotes.Result);
        gameUi.ShowMessage($"<b>{player.PlayerName}</b>, You are on trial for conspiracy against the town. <color=#2ecc71>What is your defense?</color>",
            15f, () => this.HasEnded);
    }

    protected override void Exit() { }

    protected override string GetStateInfo() => null;

    public override bool Enabled()
    {
        return trialVotes.Result != -1;
    }
}