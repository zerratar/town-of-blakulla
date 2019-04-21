public class BlackmailerRole : Role
{
    public BlackmailerRole()
        : base("Blackmailer",
            "Mafia",
            "An eavesdropper who uses information to keep people quiet.",
            "Choose one person each night to blackmail.",
            "Blackmailed targets can not talk during the day. You can hear private messages. If there are no kill-capable Mafia roles left you will become a Mafioso. You can talk with the other Mafia at night.")
    {
    }

    protected override bool CanUseAbility()
    {
        return false;
    }

    protected override void UseAbility()
    {
    }
}