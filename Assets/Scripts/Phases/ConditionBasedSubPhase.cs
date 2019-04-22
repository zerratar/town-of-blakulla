using System.Text;

public abstract class ConditionBasedSubPhase : SubPhase
{
    protected ConditionBasedSubPhase(string name)
        : base(name)
    {
    }

    protected abstract bool OnCondition(SubPhase phase, GameState state);

    public override void Update(GameState state)
    {
        if (!this.IsActive) return;
        if (OnCondition(this, state))
        {
            this.OnExit();
        }
    }

    protected override void OnReset() { }

    protected abstract string GetDebugInfo();

    public override string ToDisplayString(Phase phase)
    {
        var result = new StringBuilder();

        result.Append($"{phase.Name} - {Name} ");

        var value = GetDebugInfo();
        if (!string.IsNullOrEmpty(value))
            result.Append(value);

        return result.ToString();
    }
}