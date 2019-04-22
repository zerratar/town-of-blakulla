using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public WaypointCamera Camera;

    public PlayerController[] Players;

    private int CurrentPhaseIndex = 0;

    private GamePhaseValidator validator;

    private OldPhase[] oldphases;

    private GameMode mode = GameMode.Development;

    public Light SunLight;

    public Transform GallowPoint;


    private int phasePassed = 0;

    private float dayCycleFadeLength = 1f;
    private float dayCycleTimer = 0f;

    private Color startPlayerColor;
    private Color targetPlayerColor;

    private Color targetSkyColor;
    private float targetSunIntensity;
    private Color targetSunColor;
    private Color startSkyColor;
    private float startSunIntensity;
    private Color startSunColor;

    private TrialVoteHandler trialVoteHandler;
    private JudgementVoteHandler judgementVoteHandler;

    public TextMeshProUGUI LabelPhase;
    public TextMeshProUGUI LabelDays;

    internal int daysPassed = -1;
    private PlayerHandler playerHandler;
    private Phase[] phases;
    private StartPhase startPhase;
    private EndPhase endPhase;
    private GameUI gameUI;

    public Phase[] Phases
    {
        get
        {
            if (phases != null) return phases;
            var useStandard = this.mode == GameMode.Standard;
            return phases = new Phase[]
            {
                new DayPhase(
                    onEnter: OnEnter,
                    onExit: OnExit,
                    this.gameUI,
                    this.Camera,
                    this.playerHandler,
                    this.trialVoteHandler,
                    this.judgementVoteHandler,
                    useStandard),

                new NightPhase(
                    onEnter: OnEnter,
                    onExit: OnExit,
                    this.gameUI,
                    this.trialVoteHandler,
                    this.playerHandler,
                    useStandard),
            };
        }
    }

    public bool IsVoteForTrial => this.trialVoteHandler.CanVote;

    public bool IsVoteForJudgement => judgementVoteHandler.CanVote;

    public bool CanVote => judgementVoteHandler.CanVote || this.trialVoteHandler.CanVote;

    void Start()
    {
        this.validator = new GamePhaseValidator();
        this.trialVoteHandler = new TrialVoteHandler(this);
        this.judgementVoteHandler = new JudgementVoteHandler(this);
        if (!SunLight) SunLight = GameObject.Find("Directional Light").GetComponent<Light>();

        this.startPhase = new StartPhase();
        this.endPhase = new EndPhase();
        this.gameUI = this.GetComponent<GameUI>();

        if (!playerHandler)
        {
            playerHandler = this.GetComponent<PlayerHandler>();
        }

        if (Players == null || Players.Length == 0)
        {
            Players = GameObject.FindObjectsOfType<PlayerController>();
        }

        if (!Camera) Camera = GameObject.FindObjectOfType<WaypointCamera>();
    }

    public void OnExit(Phase phase)
    {
    }

    public void OnEnter(Phase phase)
    {
        Camera.MoveToNextWaypoint(5f);

        UpdateDayNightCycle(phase);
    }

    private void UpdateDayNightCycle(Phase phase)
    {
        this.startSkyColor = RenderSettings.ambientSkyColor;
        this.startSunIntensity = this.SunLight.intensity;
        this.startSunColor = this.SunLight.color;

        this.startPlayerColor = this.Players[0].Color;

        if (phase is DayPhase)
        {
            ++this.daysPassed;

            this.trialVoteHandler.Reset();
            this.judgementVoteHandler.Reset();

            this.targetSkyColor = new Color(0x36 / 255f, 0x3A / 255f, 0x42 / 255f, 1f);
            this.targetSunIntensity = 1f;
            this.targetSunColor = new Color(0xFF / 255f, 0xF4 / 255f, 0xD6 / 255f);
            this.targetPlayerColor = Color.white;

            LabelDays.text = this.daysPassed.ToString();
        }
        else
        {
            this.targetSkyColor = new Color(0x37 / 255f, 0x3B / 255f, 0x44 / 255f, 1f);
            this.targetSunIntensity = 0.1f;
            this.targetSunColor = new Color(0x11 / 255f, 0x2A / 255f, 0x7D / 255f);
            this.targetPlayerColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        }

        this.dayCycleTimer = this.dayCycleFadeLength;
    }


    public void JudgePlayer(string result, int voteCount)
    {
        if (this.trialVoteHandler.Result == -1)
        {
            Debug.Log($"The vote has ended with a missing player?!??!");
            return;
        }

        var player = playerHandler.GetPlayerByIndex(this.trialVoteHandler.Result);

        if (string.IsNullOrEmpty(result))
        {
            Debug.Log($"The vote has ended with not enough votes.");
            return;
        }

        if (result == JudgementVoteHandler.VoteGuilty)
        {
            Debug.Log($"The vote has ended and found {player.PlayerName} guilty.");
        }
        else
        {
            Debug.Log($"The vote has ended and found {player.PlayerName} not guilty!");
        }
    }

    public void PutPlayerOnTrialPlayer(int playerIndex, int voteCount)
    {
        if (playerIndex == -1)
        {
            Debug.Log($"The vote has ended and no one was put on trial.");
        }
        else
        {
            var player = playerHandler.GetPlayerByIndex(playerIndex);
            player.NavigateTo(GallowPoint.position);
            Debug.Log($"The vote has ended and {player.PlayerName} was voted for a trial.");
        }
    }

    // Update is called once per frame,
    void Update()
    {
        if (Players == null)
        {
            return;
        }

        if (trialVoteHandler.CanVote)
        {
            for (var i = 0; i < 8; ++i)
            {
                var key = KeyCode.Alpha0 + i;
                if (Input.GetKeyUp(key))
                {
                    var playerIndex = i - 1;

                    var index = Mathf.FloorToInt(this.Players.Length * UnityEngine.Random.value);

                    CastVoteByPlayerIndex(index, playerIndex);
                    break;
                }
            }
        }

        if (dayCycleTimer > 0f)
        {
            dayCycleTimer -= Time.deltaTime;

            var proc = 1f - (dayCycleTimer / dayCycleFadeLength);

            var playerColor = Color.Lerp(startPlayerColor, targetPlayerColor, proc);

            Players.ForEach(x => x.Color = playerColor);

            RenderSettings.ambientSkyColor = Color.Lerp(startSkyColor, targetSkyColor, proc);
            if (this.SunLight)
            {
                this.SunLight.intensity = Mathf.Lerp(startSunIntensity, targetSunIntensity, proc);
                this.SunLight.color = Color.Lerp(startSunColor, targetSunColor, proc);
            }
        }

        var phase = GetCurrentPhase();

        phase.Update(this);

        GoToNextPhaseIfEnded();

        UpdatePhaseLabel();
    }

    private void GoToNextPhaseIfEnded()
    {
        var phase = GetCurrentPhase();

        if (phase is StartPhase startPhase)
            return;

        if (phase is EndPhase endPhase)
            return;

        if (phase.HasEnded)
        {
            this.CurrentPhaseIndex = (this.CurrentPhaseIndex + 1) % this.Phases.Length;
            phases[this.CurrentPhaseIndex].Reset();
        }
    }

    public Phase GetCurrentPhase()
    {
        if (!this.startPhase.HasEnded)
        {
            return this.startPhase;
        }        

        if (this.CurrentPhaseIndex < 0 || this.CurrentPhaseIndex >= this.Phases.Length)
            return null;

        return this.Phases[this.CurrentPhaseIndex];
    }

    public bool CastVoteByPlayerIndex(int requestingPlayerIndex, int targetPlayerIndex)
    {
        return this.trialVoteHandler.CastVoteByPlayerIndex(requestingPlayerIndex, targetPlayerIndex);
    }

    public bool CastJudgementVote(int myPlayerIndex, string choice)
    {
        return this.judgementVoteHandler.CastVote(choice, myPlayerIndex);
    }

    private void UpdatePhaseLabel()
    {
        var phase = this.GetCurrentPhase();
        LabelPhase.text = phase.ToString();
    }

    public PlayerHandler GetPlayerHandler()
    {
        return this.playerHandler;
    }

    public PlayerController GetPlayerByIndex(int requestingPlayerIndex)
    {
        return playerHandler.GetPlayerByIndex(requestingPlayerIndex);
    }


    public IReadOnlyList<PlayerController> GetAliveAndAssignedPlayers()
    {
        return this.playerHandler.GetAliveAndAssignedPlayers();
    }

    public IReadOnlyList<PlayerController> GetAlivePlayers()
    {
        return this.playerHandler.GetAlivePlayers();
    }

    public IReadOnlyList<PlayerController> GetAssignedPlayers()
    {
        return this.playerHandler.GetAssignedPlayers();
    }

    public IReadOnlyList<PlayerController> GetPlayers()
    {
        return this.playerHandler.GetPlayers();
    }
}

public enum GameMode
{
    Standard,
    Rapid,
    Development
}