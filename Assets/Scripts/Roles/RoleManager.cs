using System.Collections.Concurrent;
using System.Linq;
using UnityEngine;

public class RoleManager
{
    private readonly ConcurrentDictionary<string, RoleGroup> roleGroups;

    public RoleManager()
    {
        this.roleGroups = new ConcurrentDictionary<string, RoleGroup>();
        this.AddRoleGroup(new TownRoleGroup());
        this.AddRoleGroup(new MafiaRoleGroup());
        this.AddRoleGroup(new NeutralRoleGroup());
    }

    public RoleGroup GetGroupByRole(Role role)
    {
        return this.roleGroups.FirstOrDefault(x => x.Value.HasRole(role.Name)).Value;
    }

    public RoleGroup GetRandomGroup()
    {
        var groupIndex = Mathf.FloorToInt(roleGroups.Count * UnityEngine.Random.value);
        return roleGroups.Values.ToList()[groupIndex];
    }

    public Role GetRandomRole()
    {
        return GetRandomRoleInGroup(GetRandomGroup());
    }

    public Role GetRandomRole(string groupName)
    {
        if (!roleGroups.TryGetValue(groupName, out var group))
        {
            return null;
        }

        return GetRandomRoleInGroup(group);
    }

    private Role GetRandomRoleInGroup(RoleGroup group)
    {
        var roleIndex = Mathf.FloorToInt(group.Roles.Length * UnityEngine.Random.value);
        return group.GetRoleByIndex(roleIndex);
    }

    private void AddRoleGroup(RoleGroup group)
    {
        this.roleGroups[group.Name] = group;
    }
}