using UnityEngine;

public class Player
{    
    public Player(string name)
    {
        Name = name;
    }

    public string UsernameOrName => AssignedUsername ?? Name;
    
    public string Name { get; }

    public Role AssignedRole { get; set; }

    public string AssignedUsername { get; set; }

    public Color Color { get; set; }

    public static Player Random()
    {
        var name = $"Player {Mathf.Floor(UnityEngine.Random.value * 1000)}";
        return new Player(name);
    }

    public void Reset()
    {
        AssignedUsername = null;
        AssignedRole = null;
        Color = Color.white;
    }
}