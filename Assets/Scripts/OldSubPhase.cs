using System;
using UnityEngine;

//public 

public class OldSubPhase
{
    private readonly Func<bool> isEnabled;
    private readonly Func<OldSubPhase, bool> condition;
    private bool isPhaseOver;

    public OldSubPhase(
        string key,
        float time,
        Action onEnter = null,
        Action onExit = null,
        Func<bool> isEnabled = null,
        Func<OldSubPhase, bool> condition = null,
        SubPhaseDisplaySettings settings = null)
    {
        this.Key = key;
        this.Time = time;
        this.OnEnter = onEnter;
        this.OnExit = onExit;
        this.isEnabled = isEnabled;
        this.condition = condition;
        StartTime = DateTime.MaxValue;
    }

    public string Key { get; }
    public float Time { get; }
    public Action OnEnter { get; }
    public Action OnExit { get; }

    public DateTime StartTime { get; set; }

    public DateTime Deadline => StartTime + TimeSpan.FromSeconds(this.Time);

    public bool IsOver
    {
        get
        {
            if (!IsEnabled())
            {
                return true;
            }

            if (StartTime == DateTime.MaxValue)
            {
                return true;
            }

            if (DateTime.UtcNow >= Deadline)
            {
                return true;
            }

            return InvokeCondition();
        }
    }

    public void Start()
    {
        //this.Reset();
        this.StartTime = DateTime.UtcNow;
        this.isPhaseOver = false;
        this.OnEnter?.Invoke();
    }

    public bool IsEnabled()
    {
        if (isEnabled == null) return true;
        var result = isEnabled();
        return result;
    }

    public void Reset()
    {
        this.StartTime = DateTime.MaxValue;
        this.isPhaseOver = false;
    }

    private bool InvokeCondition()
    {
        if (this.condition == null)
        {
            return false;
        }

        return this.condition(this);
    }
}

public class SubPhaseDisplaySettings
{
    public SubPhaseDisplaySettings(bool showSecondsLeft, bool showVoteCounter)
    {
        ShowSecondsLeft = showSecondsLeft;
        ShowVoteCounter = showVoteCounter;
    }

    public bool ShowSecondsLeft { get; }
    public bool ShowVoteCounter { get; }
}