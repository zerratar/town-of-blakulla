public class JanitorRole : Role
{
    public JanitorRole()
        : base("Janitor",
            "Mafia",
            "A sanitation expert working for organized crime.",
            "Choose a person to clean at night.",
            "If your target dies their role and Last Will won't be revealed to the Town. Only you will see the Cleaned targets role and Last Will. You may only perform 3 cleanings.")
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