
using System;
using System.Text;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public abstract class SubPhase
{
    protected SubPhase(string name)
    {
        Name = name;
        EnterTime = DateTime.MaxValue;
    }

    public string Name { get; }

    public bool IsActive { get; protected set; }
    public DateTime EnterTime { get; protected set; }
    public DateTime ExitTime { get; protected set; }
    public bool HasEnded { get; set; }

    public Phase Phase { get; private set; }

    protected abstract void Enter();
    protected abstract void Exit();

    public abstract bool Enabled();
    public abstract void Update(GameState state);

    public void SetPhase(Phase phase)
    {
        this.Phase = phase;
    }

    protected T GetSubPhaseOfType<T>() where T : SubPhase
    {
        if (this.Phase == null) return null;
        return this.Phase.GetSubPhaseOfType<T>();
    }


    protected void SetNextSubPhase(SubPhase votingPhase)
    {
        if (this.Phase == null) return;
        if (votingPhase == null) return;        
        this.Phase.SetNextSubPhase(votingPhase);
    }

    public void OnEnter()
    {
        //Debug.Log($"Enter Sub Phase: {this.Name}");
        // only set a start time if we have resetted the phase
        // so we can re-use a phase. Eg. voting        
        this.EnterTime = DateTime.UtcNow;

        this.HasEnded = false;
        this.IsActive = true;
        this.Enter();
    }

    public void OnExit()
    {
        //Debug.Log($"Exit Sub Phase: {this.Name}");
        this.IsActive = false;
        this.HasEnded = true;
        this.ExitTime = DateTime.UtcNow;
        this.Exit();
    }

    public void Reset()
    {
        this.IsActive = false;
        this.HasEnded = false;
        this.EnterTime = DateTime.MaxValue;
        this.ExitTime = DateTime.MaxValue;
        this.OnReset();
        this.AfterReset();
    }

    protected abstract void OnReset();

    protected virtual void AfterReset()
    {
    }

    public abstract string ToDisplayString(Phase phase);
}
