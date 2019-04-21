public class TownRoleGroup : RoleGroup
{
    public TownRoleGroup()
        : base(
            "Town",
            new MayorRole(),
            new BodyGuardRole(),
            new JailorRole(),
            new DoctorRole())
    {
    }
}