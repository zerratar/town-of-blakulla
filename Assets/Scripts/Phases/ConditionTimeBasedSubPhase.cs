using System;
using System.Text;
using UnityEngine;

public abstract class ConditionTimeBasedSubPhase : SubPhase
{
    protected ConditionTimeBasedSubPhase(
        string name,
        float duration) : base(name)
    {
        this.Duration = duration;
        this.Timer = this.Duration;
    }

    public override void Update(GameState state)
    {
        if (!this.IsActive) return;

        if (this.Timer > 0)
        {
            this.Timer -= Time.deltaTime;
        }

        if (this.Timer <= 0f || OnCondition(this, state))
        {
            this.OnExit();
        }
    }

    protected abstract bool OnCondition(SubPhase phase, GameState state);

    protected abstract string GetStateInfo();

    protected override void OnReset()
    {
        Timer = this.Duration;
    }

    public override string ToDisplayString(Phase phase)
    {
        var secondsLeft = Mathf.FloorToInt(this.Timer);

        var result = new StringBuilder();

        result.Append($"{phase.Name} - {Name} ({secondsLeft + 1}s) ");

        var value = GetStateInfo();
        if (!string.IsNullOrEmpty(value))
            result.Append(value);

        return result.ToString();
    }

    public float Timer {get; private set;} = -1f;

    public float Duration { get; }
}