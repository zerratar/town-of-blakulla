public class TownRoleGroup : RoleGroup
{
    public TownRoleGroup()
        : base(
            "Town",
            new MayorRole(),
            new BodyguardRole(),
            new JailorRole(),
            new DoctorRole())
    {
    }
}