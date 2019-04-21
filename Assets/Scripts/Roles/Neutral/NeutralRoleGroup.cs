public class NeutralRoleGroup : RoleGroup
{
    public NeutralRoleGroup()
        : base(
            "Neutral",
            new JesterRole(),
            new SurvivorRole(),
            new SerialKillerRole())
    {
    }
}