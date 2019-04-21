public class MafiaRoleGroup : RoleGroup
{
    public MafiaRoleGroup()
        : base(
            "Mafia",
            new GodfatherRole(),
            new MafiosoRole(),
            new BlackmailerRole(),
            new JanitorRole())
    {
    }
}