using UnityEngine;

public abstract class PacketHandler
{
    protected PacketHandler(GameServer server, PlayerHandler playerHandler, GameState gameState)
    {
        this.Server = server;
        this.PlayerHandler = playerHandler;
        this.GameState = gameState;
    }

    protected GameServer Server { get; }
    protected PlayerHandler PlayerHandler { get; }
    protected GameState GameState { get; }

    public abstract void Handle(Packet packet, string command, string commandArgs, string playerName, Color playerColor);
}