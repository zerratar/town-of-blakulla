using System.Linq;
using UnityEngine;

public class UseAbility : PacketHandler
{
    public UseAbility(GameServer server, PlayerHandler playerHandler, GameState gameState)
        : base(server, playerHandler, gameState)
    {
    }

    public override void Handle(Packet packet, string command, string commandArgs, string playerName, Color playerColor)
    {
        var player = PlayerHandler.FindPlayer(playerName);
        if (!player)
        {
            Server.Send(packet.CorrelationId, playerName, $"ability_failed|Can't find a player with name '{playerName}'");
            return;
        }

        var targetPlayers = new PlayerController[0];
        if (!string.IsNullOrEmpty(commandArgs))
        {
            var playerNames = commandArgs.Split(',');
            targetPlayers = playerNames.Select(x => PlayerHandler.FindPlayer(x)).Where(x => x != null).ToArray();
        }

        if (!player.Role.CanUseAbility(player, GameState, targetPlayers))
        {
            Server.Send(packet.CorrelationId, playerName, $"ability_failed|Unable to use our ability at this time.");
            return;
        }

        player.Role.UseAbility(player, targetPlayers);
        Server.Send(packet.CorrelationId, playerName, $"ability|Magic happened");
    }
}