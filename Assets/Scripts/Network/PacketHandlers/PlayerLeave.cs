using System;
using UnityEngine;

public class PlayerLeave : PacketHandler
{
    public PlayerLeave(GameServer server, PlayerHandler playerHandler, GameState gameState)
        : base(server, playerHandler, gameState)
    {
    }

    public override void Handle(Packet packet, string command, string commandArgs, string playerName, Color playerColor)
    {
        try
        {
            var playerIndex = PlayerHandler.FindPlayerIndex(playerName);
            if (playerIndex == -1)
            {
                Server.Send(packet.CorrelationId, playerName,
                    $"leave_failed|You are not assigned to a character!");
                return;
            }

            PlayerHandler.Leave(playerIndex);
            Server.Send(packet.CorrelationId, playerName, $"leave|{playerIndex}");
        }
        catch (Exception exc)
        {
            Debug.LogError("Unable leave player!!: " + exc);
        }
    }
}