using System;

public class LastWordsPhase : TimeBasedSubPhase
{
    private readonly GameUI gameUi;
    private readonly JudgementVoteHandler judgementVotes;

    public LastWordsPhase(
        GameUI gameUI,
        JudgementVoteHandler judgementVotes,
        float duration)
        : base("Last words", duration)
    {
        gameUi = gameUI;
        this.judgementVotes = judgementVotes;
    }

    protected override void Enter()
    {
        gameUi.ShowMessage("Do you have any last words?", this.Duration, () => HasEnded);
    }

    protected override void Exit()
    {
        //this.gameUi.SetMessagePositionGallows();
    }

    protected override string GetStateInfo() => null;

    public override bool Enabled()
    {
        return judgementVotes.IsGuilty;
    }
}