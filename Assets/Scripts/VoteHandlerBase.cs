using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class VoteHandlerBase<TKey>
{
    protected readonly GameState gameState;

    private readonly Dictionary<int, TKey> votes = new Dictionary<int, TKey>();

    public int VoteCount { get; private set; }

    public bool CanVote { get; private set; }

    public TKey Result { get; private set; }

    public bool HasResult => !Equals(Result, EmptyResult());


    private bool Equals(TKey a, TKey b)
    {
        var obj = a as object;
        return a != null && a.Equals(b);
    }

    protected VoteHandlerBase(GameState gameState)
    {
        this.gameState = gameState;
    }

    public void Reset()
    {
        this.Result = EmptyResult();
        this.votes.Clear();
    }

    public void BeginVoting()
    {
        this.Result = EmptyResult();
        this.votes.Clear();
        this.VoteCount = 0;
        this.CanVote = true;
    }

    public void EndVoting()
    {
        CanVote = false;

        var requiredVotes = GetRequiredVoteCount();

        var voteValues = votes
            .GroupBy(x => x.Value)
            .Select(x => new { Target = x.Key, VoteCount = x.Count() }).ToList();

        var toBeLynched = voteValues
            .FirstOrDefault(x => x.VoteCount > requiredVotes);

        var totalVoteCount = voteValues.Sum(x => x.VoteCount);

        if (toBeLynched != null)
        {
            Result = toBeLynched.Target;
            VoteEnded(Result, toBeLynched.VoteCount, totalVoteCount);
            return;
        }

        Result = EmptyResult();
        VoteEnded(Result, 0, totalVoteCount);
    }

    public int GetRequiredVoteCount()
    {
        return GetPossibleVoteCount() >> 1;
    }

    public int GetPossibleVoteCount()
    {
        return this.gameState.GetAliveAndAssignedPlayers().Count;
    }

    public int GetVoteCount()
    {
        return votes
            .GroupBy(x => x.Value)
            .Select(x => new { Target = x.Key, VoteCount = x.Count() })
            .Sum(x => x.VoteCount);
    }

    protected abstract TKey EmptyResult();

    protected abstract void VoteEnded(TKey target, int voteCount, int totalVoteCount);

    public bool CastVote(TKey target, int requestPlayerIndex)
    {
        var changedVote = votes.ContainsKey(requestPlayerIndex);

        var playerA = this.gameState.GetPlayerByIndex(requestPlayerIndex);

        Debug.Log(changedVote
            ? $"{playerA.PlayerName} changed their vote to {target}"
            : $"{playerA.PlayerName} voted for {target}");

        votes[requestPlayerIndex] = target;
        VoteCount = votes.Keys.Count;
        return changedVote;
    }

    public void CancelVote(int requestPlayerIndex)
    {
        if (!this.votes.Remove(requestPlayerIndex))
        {
            return;
        }

        var playerA = this.gameState.GetPlayerByIndex(requestPlayerIndex);
        VoteCount = votes.Keys.Count;
        Debug.Log($"{playerA.PlayerName} cancelled their vote.");
    }
}
