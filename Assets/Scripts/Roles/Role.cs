using UnityEngine;

public abstract class Role
{
    public string Name { get; }
    public string Alignment { get; }
    public string Summary { get; }
    public string Ability { get; }
    public string Attribute { get; }
    public bool Unique { get; }

    public string AlignmentColor
    {
        get
        {
            switch (Alignment)
            {
                case "Mafia": return "#e74c3c";
                case "Neutral": return "#ffffff";
                // case "Town": 
                default: return "#2ecc71";
            }
        }
    }

    protected Role(
        string name,
        string alignment,
        string summary,
        string ability,
        string attribute,
        bool unique = true)
    {
        this.Name = name;
        this.Alignment = alignment;
        this.Summary = summary;
        this.Ability = ability;
        this.Attribute = attribute;
        this.Unique = unique;
    }

    public abstract bool CanUseAbility(PlayerController player, GameState gameState, PlayerController[] targets);
    public abstract void UseAbility(PlayerController player, PlayerController[] targets);
}