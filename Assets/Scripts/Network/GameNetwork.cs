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

        server.Register<UseAbility>("ability");
        server.Register<LastWillUpdate>("last-will");
        server.Register<DeathNoteUpdate>("death-note");
        server.Register<PlayerJoin>("join");
        server.Register<PlayerLeave>("leave");
        server.Register<PlayerVote>("vote");

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

        server.HandleNextPacket(server, playerHandler, gameState);
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");

        var client = this.server.Client;
        if (client == null) return;
        var request = new Request<GameExit>();
        request.Type = "exit";
        request.Data = new GameExit();
        this.server.SendObject(request);

        server.Stop();
    }

    private void SendUpdatePacket()
    {
        var client = this.server.Client;
        if (client == null) return;
        var gameStateData = GetGameStateRequest();
        this.server.SendObject(gameStateData);
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PlayerInfo MapPlayerInfo(PlayerController x)
    {
        return new PlayerInfo
        {
            Lynched = x.Dead,
            Name = x.PlayerName,
            Role = x.Role,
            Blackmailed = x.Blackmailed,
            TargetByGodfather = x.TargetByGodfather,
            TargetByMafioso = x.TargetByMafioso,
            Cleaned = x.Cleaned,
            Healed = x.Healed,
            Jailed = x.Jailed,
            RevealedAsMayor = x.RevealedAsMayor,
        };
    }
}
