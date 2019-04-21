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

    protected override bool CanUseAbility()
    {
        return false;
    }

    protected override void UseAbility()
    {
    }
}