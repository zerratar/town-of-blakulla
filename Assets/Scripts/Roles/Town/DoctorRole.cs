public class DoctorRole : Role
{
    public DoctorRole()
        : base("Doctor",
            "Town",
            "A surgeon skilled in trauma care who secretly heals people.",
            "Heal one person each night, preventing them from dying.",
            "You may only Heal yourself once. You will know if your target is attacked. Healing will give a Powerful Defense.")
    {
    }

    public override bool CanUseAbility(PlayerController player, GameState gameState, PlayerController[] targets)
    {
        return !player.Dead && gameState.IsNight
               && targets != null && targets.Length > 0
               && (targets[0] != player || targets[0].HealCounter == 0);
    }

    public override void UseAbility(PlayerController player, PlayerController[] targets)
    {
        targets[0].Heal();
    }
}