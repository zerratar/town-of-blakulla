using System;
using UnityEngine;

public class OldPhase
{
    public int currentSubPhaseIndex = 0;

    public OldPhase(
        string name,
        params OldSubPhase[] sub)
    {
        this.Name = name;
        this.Sub = sub;
    }

    public string Name { get; }

    public DateTime StartTime { get; private set; }

    public OldSubPhase[] Sub { get; }

    public OldSubPhase CurrentSubPhase => this.Sub[currentSubPhaseIndex].StartTime <= DateTime.MaxValue
        ? this.Sub[currentSubPhaseIndex]
        : null;

    public bool IsLastSubPhase => this.currentSubPhaseIndex == this.Sub.Length - 1;

    public void MoveToNextSubPhase()
    {
        if (this.currentSubPhaseIndex != -1 && CurrentSubPhase != null)
        {
            this.Sub[this.currentSubPhaseIndex].OnExit?.Invoke();
        }

        var nextPhaseIndex = (this.currentSubPhaseIndex + 1) % this.Sub.Length;
        var subPhase = this.Sub[nextPhaseIndex];
        this.currentSubPhaseIndex = nextPhaseIndex;

        Debug.Log($"Set sub phase index: {this.currentSubPhaseIndex} - {subPhase.Key}");

        if (!subPhase.IsEnabled())
        {
            return;
        }

        EnterSubPhase(subPhase);
    }

    private static void EnterSubPhase(OldSubPhase subPhase)
    {
        subPhase.Start();
    }

    public void OnEnter()
    {
        this.StartTime = DateTime.UtcNow;
        this.currentSubPhaseIndex = -1;
        this.Sub.ForEach(x => x.Reset());
        this.MoveToNextSubPhase();
    }

    public void OnExit()
    {
        this.StartTime = DateTime.MaxValue;
    }
}