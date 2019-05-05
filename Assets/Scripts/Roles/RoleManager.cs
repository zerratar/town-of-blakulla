using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoleManager
{
    private readonly ConcurrentDictionary<string, RoleGroup> roleGroups;

    private readonly HashSet<string> assignedRoles
        = new HashSet<string>();

    private readonly ConcurrentDictionary<string, int> roleCount
        = new ConcurrentDictionary<string, int>();

    public RoleManager()
    {
        this.roleGroups = new ConcurrentDictionary<string, RoleGroup>();
        this.AddRoleGroup(new TownRoleGroup());
        this.AddRoleGroup(new MafiaRoleGroup());
        this.AddRoleGroup(new NeutralRoleGroup());
    }

    public void ResetAssignedRoles()
    {
        assignedRoles.Clear();
        roleCount.Clear();
    }

    //public RoleGroup GetGroupByRole(Role role)
    //{
    //    return this.roleGroups.FirstOrDefault(x => x.Value.HasRole(role.Name)).Value;
    //}

    public RoleGroup GetRandomGroup()
    {
        var groups = roleGroups.Values.ToList();
        var groupIndex = Mathf.FloorToInt(roleGroups.Count * UnityEngine.Random.value);
        var group = groups[groupIndex];

        roleCount.TryGetValue(group.Name, out var count);

        while (count >= 3)
        {
            groupIndex = Mathf.FloorToInt(roleGroups.Count * UnityEngine.Random.value);
            group = groups[groupIndex];
            roleCount.TryGetValue(group.Name, out count);
        }

        roleCount[group.Name] = count + 1;
        return group;
    }

    public Role GetRandomRole()
    {
        return GetRandomRoleInGroup(GetRandomGroup());
    }

    //public Role GetRandomRole(string groupName)
    //{
    //    if (!roleGroups.TryGetValue(groupName, out var group))
    //    {
    //        return null;
    //    }

    //    return GetRandomRoleInGroup(group);
    //}

    private Role GetRandomRoleInGroup(RoleGroup group)
    {
        var roleIndex = Mathf.FloorToInt(group.Roles.Length * UnityEngine.Random.value);
        var role = group.GetRoleByIndex(roleIndex);
        while (role.Unique && assignedRoles.Contains(role.Name))
        {
            roleIndex = Mathf.FloorToInt(group.Roles.Length * UnityEngine.Random.value);
            role = group.GetRoleByIndex(roleIndex);
        }
        return role;
    }

    private void AddRoleGroup(RoleGroup group)
    {
        this.roleGroups[group.Name] = group;
    }
}