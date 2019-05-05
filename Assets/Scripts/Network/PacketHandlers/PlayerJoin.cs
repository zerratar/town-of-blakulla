using System;
using UnityEngine;

public class PlayerJoin : PacketHandler
{
    public PlayerJoin(GameServer server, PlayerHandler playerHandler, GameState gameState)
        : base(server, playerHandler, gameState)
    {
    }

    public override void Handle(Packet packet, string command, string commandArgs, string playerName, Color playerColor)
    {
        try
        {
            var availablePlayers = PlayerHandler.GetAvailablePlayerSlots();

            if (availablePlayers.Count == 0)
            {
                Server.Send(packet.CorrelationId, playerName, $"join_failed|No player slots available.");
                return;
            }

            var playerIndex = PlayerHandler.FindPlayerIndex(playerName);
            if (playerIndex != -1)
            {
                Server.Send(packet.CorrelationId, playerName, $"join_failed|You've already joined the game!");
                return;
            }

            var playerSlot = availablePlayers[Mathf.FloorToInt(UnityEngine.Random.value * availablePlayers.Count)];

            var player = PlayerHandler.Assign(playerSlot, playerName, playerColor);

            Server.Send( packet.CorrelationId, playerName, $"join|{playerSlot}|{player.Role.Name}");

            Debug.Log($"Player: {playerName} joined on slot {playerSlot}");
        }
        catch (Exception exc)
        {
            Debug.LogError("Unable to assign player!!: " + exc);
        }
    }
}