using System.Linq;

public class BlackmailerRole : Role
{
    public BlackmailerRole()
        : base("Blackmailer",
            "Mafia",
            "An eavesdropper who uses information to keep people quiet.",
            "Choose one person each night to blackmail.",
            "Blackmailed targets can not talk during the day. " +
                    "You can hear private messages. " +
                    "If there are no kill-capable Mafia roles left you will become a Mafioso. " +
                    "You can talk with the other Mafia at night.")
    {
    }

    public override bool CanUseAbility(PlayerController player, GameState gameState, PlayerController[] targets)
    {
        return !player.Dead && gameState.IsNight && targets != null && targets.Length > 0
               && targets.All(x => x != player && x.Role.Alignment != Alignment && x.Dead);
    }

    public override void UseAbility(PlayerController player, PlayerController[] targets)
    {
        targets[0].Blackmailed = true;
    }
}