using System.Linq;

public class JanitorRole : Role
{
    public JanitorRole()
        : base("Janitor",
            "Mafia",
            "A sanitation expert working for organized crime.",
            "Choose a person to clean at night.",
            "If your target dies their role and Last Will won't be revealed to the Town. " +
                    "Only you will see the Cleaned targets role and Last Will. " +
                    "You may only perform 3 cleanings.")
    {
    }

    public override bool CanUseAbility(PlayerController player, GameState gameState, PlayerController[] targets)
    {
        return !player.Dead && gameState.IsNight && targets != null && targets.Length > 0
            && targets.All(x => x != player && x.Role.Alignment != Alignment && x.Dead);
    }

    public override void UseAbility(PlayerController player, PlayerController[] targets)
    {
        targets[0].Cleaned = true;
    }
}