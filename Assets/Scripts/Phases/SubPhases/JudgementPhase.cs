using System;

public class JudgementPhase : ConditionTimeBasedSubPhase
{
    private readonly GameUI gameUi;
    private readonly PlayerHandler playerHandler;
    private readonly TrialVoteHandler trialVotes;
    private readonly JudgementVoteHandler judgementVotes;

    public JudgementPhase(
        GameUI gameUI,
        PlayerHandler playerHandler,
        TrialVoteHandler trialVotes,
        JudgementVoteHandler judgementVotes,
        float duration)
        : base("Judgement", duration)
    {
        gameUi = gameUI;
        this.playerHandler = playerHandler;
        this.trialVotes = trialVotes;
        this.judgementVotes = judgementVotes;
    }

    protected override void Enter()
    {
        var player = playerHandler.GetPlayerByIndex(trialVotes.Result);

        gameUi.ShowMessage($"The town may now vote on the fate of <b>{player.PlayerName}</b>.",
            this.Duration, () => this.HasEnded);

        judgementVotes.BeginVoting();
    }

    protected override void Exit()
    {
        judgementVotes.EndVoting();
        this.ResetConditionsAndTimers();
        if (judgementVotes.HasResult && judgementVotes.IsGuilty)
        {
            var player = playerHandler.GetPlayerByIndex(trialVotes.Result);
            var voteCountFor = judgementVotes.VoteCountFor;
            var maxVoteCount = judgementVotes.VoteCount;
            gameUi.ShowMessage(
                $"The town has decided to lynch <b>{player.PlayerName}</b> by a vote of <color=#e74c3c>{voteCountFor}</color> for <color=#2ecc71>{maxVoteCount - voteCountFor}</color>.",
                2f);
        }
    }

    public override bool Enabled()
    {
        return trialVotes.Result != -1;
    }

    protected override bool OnCondition(SubPhase phase, GameState state)
    {
        var requiredVotes = judgementVotes.GetRequiredVoteCount();
        var voteCount = judgementVotes.VoteCount;
        var result = voteCount > requiredVotes;
        return result;
    }

    protected override string GetStateInfo()
    {
        return $"{judgementVotes.VoteCount}/{playerHandler.GetAssignedPlayers().Count} voted.";
    }
}