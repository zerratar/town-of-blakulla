using System;
using System.Linq;

public abstract class RoleGroup
{
    protected RoleGroup(
        string name,
        params Role[] roles)
    {
        Name = name;
        this.Roles = roles;
    }

    public string Name { get; }
    public Role[] Roles { get; }

    public Role GetRoleByIndex(int roleIndex)
    {
        if (roleIndex < 0 || roleIndex >= this.Roles.Length)
        {
            return null;
        }

        return this.Roles[roleIndex];
    }

    public bool HasRole(string roleName)
    {
        return this.Roles.Any(x =>
            x.Name.Equals(roleName, StringComparison.InvariantCultureIgnoreCase));
    }
}