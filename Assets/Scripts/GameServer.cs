using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Newtonsoft.Json;
using UnityEditor.PackageManager;
using UnityEngine;

public class GameServer : IDisposable
{
    private readonly TcpListener server;

    private readonly List<GameClient> connectedClients
        = new List<GameClient>();

    private readonly ConcurrentQueue<Packet> availablePackets
        = new ConcurrentQueue<Packet>();

    private readonly ConcurrentDictionary<string, Type> packetHandlers
        = new ConcurrentDictionary<string, Type>();

    public GameServer()
    {
        var ipEndPoint = new IPEndPoint(IPAddress.Any, 6000);
        this.server = new TcpListener(ipEndPoint);
    }

    public bool IsBound => this.server.Server.IsBound;

    public void Start()
    {
        this.server.Start(0x1000);
        this.server.BeginAcceptTcpClient(OnAcceptTcpClient, null);
        Debug.Log("Server started");
    }

    public GameClient Client => this.connectedClients.Count > 0
            ? this.connectedClients.FirstOrDefault(x => x.Connected)
            : null;

    public Packet ReadPacket()
    {
        if (this.availablePackets.TryDequeue(out var packet))
        {
            Debug.Log("Read packet: " + packet.Command);
            return packet;
        }
        return null;
    }

    public void HandleNextPacket(params object[] args)
    {
        var packet = ReadPacket();
        if (string.IsNullOrEmpty(packet?.Command))
        {
            return;
        }

        HandlePacket(packet, args);
    }

    public void Stop()
    {
        if (this.server.Server.IsBound)
        {
            this.server.Stop();
        }
    }

    public void Dispose()
    {
        this.Stop();
    }

    public void DataReceived(GameClient gameClient, string rawCommand)
    {
        var correlationEnd = rawCommand.IndexOf('|');
        var correlationId = rawCommand.Remove(correlationEnd);
        var command = rawCommand.Substring(correlationEnd + 1);
        availablePackets.Enqueue(new Packet(gameClient, correlationId, command));
    }

    public void Register<T>(string packetCommand)
    {
        this.packetHandlers[packetCommand.ToLower()] = typeof(T);
    }

    public void SendObject<T>(T obj)
    {
        this.Client?.Write(JsonConvert.SerializeObject(obj));
    }

    public void Send(string correlationId, string playerName, string rawCommand)
    {
        this.Client?.Write(correlationId + "|" + playerName + ":" + rawCommand);
    }

    private void HandlePacket(Packet packet, params object[] packetHandlerArgs)
    {
        Debug.Log("Packet recvd: " + packet.Command);
        try
        {
            var data = packet.Command.IndexOf(':');
            var playerName = packet.Command.Remove(data);
            var playerColor = Color.white;
            if (playerName.Contains("|"))
            {
                var playerInfo = playerName.Split('|');
                playerName = playerInfo[0];
                TryParseHexColor(playerInfo[1], out playerColor);
            }

            var command = packet.Command.Substring(data + 1).Trim();
            var args = "";
            if (command.Contains("|"))
            {
                var argSplitIndex = command.IndexOf("|", StringComparison.Ordinal);
                args = command.Substring(argSplitIndex + 1);
                command = command.Remove(argSplitIndex);
            }
            else if (command.Contains(" "))
            {
                var argSplitIndex = command.IndexOf(" ", StringComparison.Ordinal);
                args = command.Substring(argSplitIndex + 1);
                command = command.Remove(argSplitIndex);
            }

            Debug.Log("We got a packet from the chat bot!: " +
                      data + ", command: " + command + ", commandArgs: " + args + ", username: " + playerName);

            if (!packetHandlers.TryGetValue(command.ToLower(), out var handlerType))
            {
                Debug.LogError($"'{command}' is not a known command. :(");
                return;
            }

            HandlePacket(handlerType, packetHandlerArgs, packet, command, args, playerName, playerColor);
        }
        catch (Exception exc)
        {
            Debug.LogError(exc.ToString());
        }
    }

    private void HandlePacket(
        Type packetHandlerType,
        object[] packetHandlerArgs,
        Packet packet,
        string command,
        string commandArgs,
        string playerName,
        Color playerColor)
    {
        var packetHandler = InstantiateHandler(packetHandlerType, packetHandlerArgs);
        if (packetHandler == null)
        {
            throw new Exception(
                $"Nooo! Packet handler for {packetHandlerType.FullName} could not be instantiated. Plz fix!");
        }

        packetHandler.Handle(packet, command, commandArgs, playerName, playerColor);
    }

    private PacketHandler InstantiateHandler(Type packetHandlerType, params object[] args)
    {
        var ctors = packetHandlerType.GetConstructors(
            BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        var ctor = ctors.FirstOrDefault();
        if (ctor == null)
        {
            Debug.LogError($"InstantiateHandler: No public constructor found!");
            return null;
        }

        var parameters = ctor.GetParameters();
        if (parameters.Length != args.Length)
        {
            Debug.LogError($"InstantiateHandler: Unexpected amount of parameters for ctor: {parameters.Length}, expected: {args.Length}");
            return null;
        }

        return (PacketHandler)ctor.Invoke(args);
    }

    private void OnAcceptTcpClient(IAsyncResult ar)
    {
        try
        {
            var tcpClient = this.server.EndAcceptTcpClient(ar);
            this.ClientConnected(tcpClient);
        }
        catch { }
        try
        {
            this.server.BeginAcceptTcpClient(OnAcceptTcpClient, null);
        }
        catch { }
    }

    private void ClientConnected(TcpClient client)
    {
        Debug.Log("Client connected");
        this.connectedClients.Add(new GameClient(this, client));
    }

    private bool TryParseHexColor(string hexColorString, out Color color)
    {
        color = Color.white;
        if (string.IsNullOrEmpty(hexColorString))
        {
            color = TwitchColors.All[Mathf.FloorToInt(TwitchColors.All.Length * UnityEngine.Random.value)];
            return false;
        }

        if (hexColorString.StartsWith("#"))
        {
            hexColorString = hexColorString.Substring(1);
        }

        var rgb = new float[3];
        for (var i = 0; i < 3; ++i)
        {
            var hex = hexColorString.Substring(i * 2, 2);
            rgb[i] = (float)int.Parse(hex, System.Globalization.NumberStyles.HexNumber) / 255f;
        }

        color = new Color(rgb[0], rgb[1], rgb[2], 1f);
        return true;
    }

}