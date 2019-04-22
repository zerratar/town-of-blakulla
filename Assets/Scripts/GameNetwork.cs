using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public class GameNetwork : MonoBehaviour
{
    private GameServer server = new GameServer();

    private PlayerHandler playerHandler;

    private GameState gameState;


    private float gameUpdateTimer = 0.5f;
    private float gameUpdateInterval = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        if (!playerHandler) playerHandler = this.GetComponent<PlayerHandler>();
        if (!gameState) gameState = this.GetComponent<GameState>();
        server.Start();
    }


    // Update is called once per frame
    void Update()
    {
        if (!server.IsBound)
        {
            return;
        }

        gameUpdateTimer -= Time.deltaTime;
        if (gameUpdateTimer <= 0f)
        {
            gameUpdateTimer = gameUpdateInterval;
            SendUpdatePacket();
        }

        var packet = server.ReadPacket();
        if (string.IsNullOrEmpty(packet?.Command))
        {
            return;
        }

        HandlePacket(packet);
    }

    private void SendUpdatePacket()
    {
        var client = this.server.Client;
        if (client == null) return;
        var gameStateData = GetGameStateRequest();

        SendObject(client, gameStateData);
    }

    private Request<GameUpdateInfo> GetGameStateRequest()
    {
        var result = new GameUpdateInfo();
        var request = new Request<GameUpdateInfo>();
        request.Type = "update";
        request.Data = result;

        result.Joinable = playerHandler.GetAvailablePlayerSlots().Count > 0;
        result.Players = playerHandler
            .GetPlayers()
            .Select(MapPlayerInfo)
            .ToList();

        result.DaysPassed = gameState.daysPassed;
        result.Started = result.DaysPassed > -1;

        var phase = gameState.GetCurrentPhase();
        if (phase == null)
        {
            return request;
        }

        result.PhaseName = phase.Name;
        var subPhase = phase.GetCurrentSubPhase();
        if (subPhase == null)
        {
            return request;
        }

        result.SubPhase.Name = subPhase.Name;
        result.SubPhase.EnterTime = subPhase.EnterTime;
        result.SubPhase.ExitTime = subPhase.ExitTime;
        result.SubPhase.HasEnded = subPhase.HasEnded;
        result.SubPhase.IsActive = subPhase.IsActive;

        if (subPhase is TimeBasedSubPhase time)
        {
            result.SubPhase.Timer = time.Timer;
            result.SubPhase.Duration = time.Duration;
        }
        else if (subPhase is ConditionTimeBasedSubPhase conditionTime)
        {
            result.SubPhase.Timer = conditionTime.Timer;
            result.SubPhase.Duration = conditionTime.Duration;
        }

        return request;
    }

    private void HandlePacket(Packet packet)
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
                      data + ", command: " + command + ", args: " + args + ", username: " + playerName);

            if (command.InvariantEquals("last-will"))
            {
                HandleLastWillUpdate(packet, playerName, args);
            }
            else if (command.InvariantEquals("death-note"))
            {
                HandleDeathNoteUpdate(packet, playerName, args);
            }
            else if (command.InvariantEquals("join"))
            {
                HandlePlayerJoin(packet, playerName, playerColor);
            }
            else if (command.InvariantEquals("leave"))
            {
                HandlePlayerLeave(packet, playerName);
            }
            else if (command.InvariantEquals("vote"))
            {
                HandlePlayerVote(packet, playerName, args);
            }
        }
        catch (Exception exc)
        {
            Debug.LogError(exc.ToString());
        }
    }

    private void HandleLastWillUpdate(Packet packet, string myPlayerName, string args)
    {
        var playerIndex = this.playerHandler.FindPlayerIndex(myPlayerName);
        if (playerIndex < 0)
        {
            Send(packet.Client, packet.CorrelationId, myPlayerName, $"last-will_changed|Player does not exist");
            return;
        }

        try
        {
            var player = this.playerHandler.GetPlayerByIndex(playerIndex);
            var bytes = Convert.FromBase64String(args);
            var lastWill = Encoding.UTF8.GetString(bytes);
            player.LastWill = lastWill;
            Debug.Log("Last will updated: " + player.LastWill);
        }
        catch (Exception exc)
        {
            Debug.LogError(exc.ToString());
        }
        finally
        {
            Send(packet.Client, packet.CorrelationId, myPlayerName, "last-will_changed|");
        }
    }

    private void HandleDeathNoteUpdate(Packet packet, string myPlayerName, string args)
    {
        var playerIndex = this.playerHandler.FindPlayerIndex(myPlayerName);
        if (playerIndex < 0)
        {
            Send(packet.Client, packet.CorrelationId, myPlayerName, $"death-note_changed|Player does not exist");
            return;
        }

        try
        {
            var player = this.playerHandler.GetPlayerByIndex(playerIndex);
            var bytes = Convert.FromBase64String(args);
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
            Send(packet.Client, packet.CorrelationId, myPlayerName, "death-note_changed|");
        }
    }

    private void HandlePlayerVote(Packet packet, string myPlayerName, string args)
    {
        try
        {
            if (!this.gameState.CanVote)
            {
                Send(packet.Client, packet.CorrelationId, myPlayerName, $"vote_failed|You cannot vote yet!");
                return;
            }

            var myPlayerIndex = this.playerHandler.FindPlayerIndex(myPlayerName);
            if (myPlayerIndex == -1)
            {
                Send(packet.Client, packet.CorrelationId, myPlayerName, $"vote_failed|You are not assigned to a character!");
                return;
            }

            var player = this.playerHandler.GetPlayerByIndex(myPlayerIndex);
            if (player.Dead)
            {
                Send(packet.Client, packet.CorrelationId, myPlayerName, $"vote_failed|U is ded.");
                return;
            }

            if (string.IsNullOrEmpty(args))
            {
                Send(packet.Client, packet.CorrelationId, myPlayerName,
                    this.gameState.IsVoteForTrial
                        ? $"vote_failed|You need to supply player number or player name. Ex: !town vote zerratar"
                        : $"vote_failed|You need to supply 'guilty' or 'innocent'. Ex: !town vote guilty");

                return;
            }

            if (this.gameState.IsVoteForTrial)
            {
                VoteForTrial(packet, myPlayerName, myPlayerIndex, args);
            }
            else
            {
                VoteForJudgement(packet, myPlayerName, myPlayerIndex, args);
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
            Send(packet.Client, packet.CorrelationId, myPlayerName,
                $"vote_failed|You need to supply a valid selection. (guilty / innocent)");
            return;
        }

        if (gameState.CastJudgementVote(myPlayerIndex, lower))
        {
            Send(packet.Client, packet.CorrelationId, myPlayerName,
                "vote_changed|Thank you for voting!");
        }
        else
        {
            Send(packet.Client, packet.CorrelationId, myPlayerName,
                "vote|Thank you for voting!");
        }
    }

    private void VoteForTrial(Packet packet, string playerName, int myPlayerIndex, string args)
    {
        var targetPlayerIndex = -1;
        if (int.TryParse(args, out var number))
        {
            if (number < 1 || number > this.playerHandler.PlayerCount)
            {
                Send(packet.Client, packet.CorrelationId, playerName,
                    $"vote_failed|You need to supply a valid player number. (1-{this.playerHandler.PlayerCount})");
                return;
            }

            if (number == myPlayerIndex + 1)
            {
                Send(packet.Client, packet.CorrelationId, playerName,
                    $"vote_failed|You can't vote on yourself.");
                return;
            }

            targetPlayerIndex = number - 1;
        }
        else
        {
            targetPlayerIndex = this.playerHandler.FindPlayerIndex(args);
        }

        if (targetPlayerIndex == myPlayerIndex)
        {
            Send(packet.Client, packet.CorrelationId, playerName,
                $"vote_failed|You can't vote on yourself.");
            return;
        }

        if (targetPlayerIndex == -1)
        {
            Send(packet.Client, packet.CorrelationId, playerName,
                $"vote_failed|Invalid player number or name.");
            return;
        }

        var targetPlayer = playerHandler.GetPlayerByIndex(targetPlayerIndex);

        if (targetPlayer.Dead)
        {
            Send(packet.Client, packet.CorrelationId, playerName,
                $"vote_failed|{targetPlayer.PlayerName} is dead.");
            return;
        }

        Send(packet.Client, packet.CorrelationId, playerName,
            gameState.CastVoteByPlayerIndex(myPlayerIndex, targetPlayerIndex)
                ? $"vote_changed|Thank you for voting!"
                : $"vote|Thank you for voting!");
    }

    private void HandlePlayerLeave(Packet packet, string playerName)
    {
        try
        {
            var playerIndex = this.playerHandler.FindPlayerIndex(playerName);
            if (playerIndex == -1)
            {
                Send(packet.Client, packet.CorrelationId, playerName,
                    $"leave_failed|You are not assigned to a character!");
                return;
            }

            this.playerHandler.Leave(playerIndex);
            Send(packet.Client, packet.CorrelationId, playerName, $"leave|{playerIndex}");
        }
        catch (Exception exc)
        {
            Debug.LogError("Unable leave player!!: " + exc);
        }
    }

    private void HandlePlayerJoin(Packet packet, string playerName, Color playerColor)
    {
        try
        {
            var availablePlayers = playerHandler.GetAvailablePlayerSlots();

            if (availablePlayers.Count == 0)
            {
                Send(packet.Client, packet.CorrelationId, playerName, $"join_failed|No player slots available.");
                return;
            }

            var playerIndex = this.playerHandler.FindPlayerIndex(playerName);
            if (playerIndex != -1)
            {
                Send(packet.Client, packet.CorrelationId, playerName, $"join_failed|You've already joined the game!");
                return;
            }

            var playerSlot = availablePlayers[Mathf.FloorToInt(UnityEngine.Random.value * availablePlayers.Count)];

            var player = playerHandler.Assign(playerSlot, playerName, playerColor);

            Send(packet.Client, packet.CorrelationId, playerName, $"join|{playerSlot}|{player.Role.Name}");

            Debug.Log($"Player: {playerName} joined on slot {playerSlot}");
        }
        catch (Exception exc)
        {
            Debug.LogError("Unable to assign player!!: " + exc);
        }
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

    private void SendObject<T>(GameClient client, T obj)
    {
        client?.Write(JsonConvert.SerializeObject(obj));
    }

    private void Send(GameClient client, string correlationId, string playerName, string rawCommand)
    {
        client?.Write(correlationId + "|" + playerName + ":" + rawCommand);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PlayerInfo MapPlayerInfo(PlayerController x)
    {
        return new PlayerInfo { Lynched = x.Dead, Name = x.PlayerName, Role = x.Role };
    }
}

public class Request<T>
{
    public string Type { get; set; }
    public T Data { get; set; }
}

public class GameUpdateInfo
{
    public GameUpdateInfo()
    {
        SubPhase = new SubPhaseInfo();
        Players = new List<PlayerInfo>();
    }

    public string PhaseName { get; set; }
    public SubPhaseInfo SubPhase { get; set; }
    public int DaysPassed { get; set; }
    public bool Started { get; set; }
    public bool Joinable { get; set; }
    public List<PlayerInfo> Players { get; set; }
}

public class PlayerInfo
{
    public string Name { get; set; }
    public bool Lynched { get; set; }
    public Role Role { get; set; }
}

public class SubPhaseInfo
{
    public string Name { get; set; }
    public float Timer { get; set; }
    public float Duration { get; set; }
    public float EnterTime { get; set; }
    public float ExitTime { get; set; }
    public bool HasEnded { get; set; }
    public bool IsActive { get; set; }
}