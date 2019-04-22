public class StartPhase : Phase
{
    public StartPhase(
        params SubPhase[] subPhases)
        : base("Start Game", subPhases)
    {
    }

    protected override void OnEnter()
    {
    }

    protected override void OnExit()
    {
    }
}
