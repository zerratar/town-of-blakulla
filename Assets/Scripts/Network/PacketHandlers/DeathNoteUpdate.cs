using System;
using System.Text;
using UnityEngine;

public class DeathNoteUpdate : PacketHandler
{
    public DeathNoteUpdate(GameServer server, PlayerHandler playerHandler, GameState gameState)
        : base(server, playerHandler, gameState)
    {
    }

    public override void Handle(Packet packet, string command, string commandArgs, string playerName, Color playerColor)
    {
        var playerIndex = PlayerHandler.FindPlayerIndex(playerName);
        if (playerIndex < 0)
        {
            Server.Send(packet.CorrelationId, playerName, $"death-note_changed|Player does not exist");
            return;
        }

        try
        {
            var player = PlayerHandler.GetPlayerByIndex(playerIndex);
            var bytes = Convert.FromBase64String(commandArgs);
            var value = Encoding.UTF8.GetString(bytes);
            player.DeathNote = value;
            Debug.Log("Death note updated: " + player.DeathNote);
        }
        catch (Exception exc)
        {
            Debug.LogError(exc.ToString());
        }
        finally
        {
            Server.Send( packet.CorrelationId, playerName, "death-note_changed|");
        }
    }
}