public abstract class Role
{
    public string Name { get; }
    public string Alignment { get; }
    public string Summary { get; }
    public string Ability { get; }
    public string Attribute { get; }

    protected Role(
        string name,
        string alignment,
        string summary,
        string ability,
        string attribute)
    {
        this.Name = name;
        this.Alignment = alignment;
        this.Summary = summary;
        this.Ability = ability;
        Attribute = attribute;
    }

    protected abstract bool CanUseAbility();
    protected abstract void UseAbility();
}