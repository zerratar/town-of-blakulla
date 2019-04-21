using System;
using System.Linq;

public class GamePhaseValidator
{
    public bool Validate(OldPhase phase, GameState state)
    {
        foreach (var x in phase.Sub)
        {
            if (!x.IsOver) return false;
        }

        return true;
    }
}