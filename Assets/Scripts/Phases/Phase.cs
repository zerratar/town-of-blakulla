using System;
using System.Linq;
using UnityEngine;

public abstract class Phase
{
    public string Name { get; }

    protected readonly SubPhase[] subPhases;

    protected int currentSubPhaseIndex = -1;

    private int nextSubPhaseIndexOverride = -1;

    private bool hasStarted;


    protected Phase(string name, params SubPhase[] subPhases)
    {
        this.Name = name;
        this.subPhases = subPhases;
        foreach (var phase in subPhases) phase.SetPhase(this);
    }

    public bool HasEnded { get; private set; }

    public void Update(GameState gameState)
    {
        if (HasEnded)
        {
            return;
        }

        if (!hasStarted)
        {
            Enter();
        }

        var subPhaseEnded = false;
        var isPhaseEnabled = false;
        if (currentSubPhaseIndex < subPhases.Length)
        {
            var phase = subPhases[currentSubPhaseIndex];

            isPhaseEnabled = phase.Enabled();

            if (isPhaseEnabled && !phase.HasEnded)
            {
                if (!phase.IsActive)
                {
                    phase.OnEnter();
                }

                phase.Update(gameState);
            }

            subPhaseEnded = phase.HasEnded;
        }

        if (!isPhaseEnabled || subPhaseEnded)
        {
            if (currentSubPhaseIndex >= subPhases.Length - 1)
            {
                Exit();
                return;
            }

            if (nextSubPhaseIndexOverride != -1)
            {
                for (var i = this.nextSubPhaseIndexOverride; i < this.subPhases.Length; ++i)
                {
                    this.subPhases[i].HasEnded = false;
                }

                currentSubPhaseIndex = nextSubPhaseIndexOverride;
                nextSubPhaseIndexOverride = -1;
            }
            else
            {
                currentSubPhaseIndex =
                    (currentSubPhaseIndex + 1) % subPhases.Length;
            }
        }
    }

    public SubPhase GetCurrentSubPhase()
    {
        if (this.currentSubPhaseIndex < 0 || this.currentSubPhaseIndex >= this.subPhases.Length)
            return null;

        return this.subPhases[this.currentSubPhaseIndex];
    }

    protected abstract void OnEnter();
    protected abstract void OnExit();

    private void Enter()
    {
        Debug.Log($"Enter Phase: {this.Name}");
        currentSubPhaseIndex = 0;
        hasStarted = true;
        HasEnded = false;
        OnEnter();
    }

    private void Exit()
    {
        Debug.Log($"Exit Phase: {this.Name}");
        currentSubPhaseIndex = -1;
        hasStarted = false;
        HasEnded = true;
        OnExit();
    }

    public override string ToString()
    {
        if (this.HasEnded || currentSubPhaseIndex == -1)
        {
            return this.Name;
        }

        var subPhase = subPhases[currentSubPhaseIndex];
        return subPhase.ToDisplayString(this);
    }

    public void Reset()
    {
        currentSubPhaseIndex = -1;
        hasStarted = false;
        HasEnded = false;
        subPhases.ForEach(x => x.Reset());
    }

    public T GetSubPhaseOfType<T>() where T : SubPhase
    {
        return (T)this.subPhases.FirstOrDefault(x => x.GetType() == typeof(T));
    }

    public void SetNextSubPhase(SubPhase votingPhase)
    {
        this.nextSubPhaseIndexOverride = Array.IndexOf(this.subPhases, votingPhase);
    }
}