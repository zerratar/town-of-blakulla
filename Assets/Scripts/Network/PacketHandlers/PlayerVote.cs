using System;
using UnityEngine;

public class PlayerVote : PacketHandler
{
    public PlayerVote(GameServer server, PlayerHandler playerHandler, GameState gameState)
        : base(server, playerHandler, gameState)
    {
    }

    public override void Handle(Packet packet, string command, string commandArgs, string playerName, Color playerColor)
    {
        try
        {
            if (!GameState.CanVote)
            {
                Server.Send(packet.CorrelationId, playerName, $"vote_failed|You cannot vote yet!");
                return;
            }

            var myPlayerIndex = PlayerHandler.FindPlayerIndex(playerName);
            if (myPlayerIndex == -1)
            {
                Server.Send(packet.CorrelationId, playerName, $"vote_failed|You are not assigned to a character!");
                return;
            }

            var player = PlayerHandler.GetPlayerByIndex(myPlayerIndex);
            if (player.Dead)
            {
                Server.Send( packet.CorrelationId, playerName, $"vote_failed|U is ded.");
                return;
            }

            if (string.IsNullOrEmpty(commandArgs))
            {
                Server.Send( packet.CorrelationId, playerName,
                    GameState.IsVoteForTrial
                        ? $"vote_failed|You need to supply player number or player name. Ex: !town vote zerratar"
                        : $"vote_failed|You need to supply 'guilty' or 'innocent'. Ex: !town vote guilty");

                return;
            }

            if (GameState.IsVoteForTrial)
            {
                VoteForTrial(packet, playerName, myPlayerIndex, commandArgs);
            }
            else
            {
                VoteForJudgement(packet, playerName, myPlayerIndex, commandArgs);
            }
        }
        catch (Exception exc)
        {
            Debug.LogError("Unable to vote!!: " + exc);
        }
    }



    private void VoteForJudgement(Packet packet, string myPlayerName, int myPlayerIndex, string args)
    {
        var lower = args?.ToLower().Trim();

        if (string.IsNullOrEmpty(lower) ||
            lower != JudgementVoteHandler.VoteGuilty && lower != JudgementVoteHandler.VoteInnocent)
        {
            Server.Send(packet.CorrelationId, myPlayerName,
                $"vote_failed|You need to supply a valid selection. (guilty / innocent)");
            return;
        }

        if (GameState.CastJudgementVote(myPlayerIndex, lower))
        {
            Server.Send(packet.CorrelationId, myPlayerName,
                "vote_changed|Thank you for voting!");
        }
        else
        {
            Server.Send(packet.CorrelationId, myPlayerName,
                "vote|Thank you for voting!");
        }
    }

    private void VoteForTrial(Packet packet, string playerName, int myPlayerIndex, string args)
    {
        var targetPlayerIndex = -1;
        if (int.TryParse(args, out var number))
        {
            if (number < 1 || number > PlayerHandler.PlayerCount)
            {
                Server.Send(packet.CorrelationId, playerName,
                    $"vote_failed|You need to supply a valid player number. (1-{PlayerHandler.PlayerCount})");
                return;
            }

            if (number == myPlayerIndex + 1)
            {
                Server.Send(packet.CorrelationId, playerName,
                    $"vote_failed|You can't vote on yourself.");
                return;
            }

            targetPlayerIndex = number - 1;
        }
        else
        {
            targetPlayerIndex = PlayerHandler.FindPlayerIndex(args);
        }

        if (targetPlayerIndex == myPlayerIndex)
        {
            Server.Send(packet.CorrelationId, playerName,
                $"vote_failed|You can't vote on yourself.");
            return;
        }

        if (targetPlayerIndex == -1)
        {
            Server.Send(packet.CorrelationId, playerName,
                $"vote_failed|Invalid player number or name.");
            return;
        }

        var targetPlayer = PlayerHandler.GetPlayerByIndex(targetPlayerIndex);

        if (targetPlayer.Dead)
        {
            Server.Send(packet.CorrelationId, playerName,
                $"vote_failed|{targetPlayer.PlayerName} is dead.");
            return;
        }

        Server.Send(packet.CorrelationId, playerName,
            GameState.CastVoteByPlayerIndex(myPlayerIndex, targetPlayerIndex)
                ? $"vote_changed|Thank you for voting!"
                : $"vote|Thank you for voting!");
    }

}