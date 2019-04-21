//public enum GamePhaseTrigger
//{
//    ReachWaypoint,
//    CompleteAllParts,
//    TimeBased
//}


using System;

public class GamePhaseTrigger { }


public class ReachWaypointTrigger : GamePhaseTrigger
{
}

public class CompleteAllPartsTrigger : GamePhaseTrigger
{
}

public class TimeBasedTrigger : GamePhaseTrigger
{
    public TimeSpan Time { get; }

    public TimeBasedTrigger(TimeSpan time)
    {
        this.Time = time;
    }
}