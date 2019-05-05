using System.Linq;
using UnityEngine.Rendering;

public class BodyguardRole : Role
{
    public BodyguardRole()
        : base(
            "Bodyguard",
            "Town",
            "An ex-soldier who secretly makes a living by selling protection.",
            "Protect a player from a direct attack at night.",
            "If your target is attacked or is the victim of a harmful visit, you and the visitor will fight. " +
                    "If you successfully protect someone you can still be Healed.")
    {
    }

    public override bool CanUseAbility(
        PlayerController player,
        GameState gameState,
        PlayerController[] targets)
    {
        return !player.Dead && gameState.IsNight && targets != null && targets.Length > 0
            && targets.All(x => x != player);
    }

    public override void UseAbility(PlayerController player, PlayerController[] targets)
    {
        player.Protectee = targets[0];
    }
}