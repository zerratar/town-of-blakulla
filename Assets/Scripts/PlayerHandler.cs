using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    private List<PlayerController> players;

    private readonly object mutex = new object();

    private RoleManager roleManager = new RoleManager();

    // Start is called before the first frame update
    void Start()
    {
        lock (mutex)
        {
            this.players = new List<PlayerController>(GameObject.FindObjectsOfType<PlayerController>());
        }
    }

    public void Leave(int index)
    {
        lock (mutex)
        {
            if (index < 0 || index >= this.players.Count) return;

            this.players[index].Leave();
        }
    }

    public PlayerController Assign(int index, string username, Color color)
    {
        lock (mutex)
        {
            if (index < 0 || index >= this.players.Count) return null;
            var role = roleManager.GetRandomRole();
            return this.players[index].Assign(username, color, role);
        }
    }

    public int PlayerCount
    {
        get
        {
            lock (mutex)
            {
                return this.players.Count;
            }
        }
    }

    public IReadOnlyList<int> GetAvailablePlayerSlots()
    {
        lock (mutex)
        {
            return players
                .Where(x => !x.IsAssigned)
                .Select(x => this.players.IndexOf(x))
                .ToList();
        }
    }

    public int FindPlayerIndex(string username)
    {
        lock (mutex)
        {
            return players.FindIndex(x =>
                x.PlayerData.UsernameOrName != null &&
                x.PlayerData.UsernameOrName.Equals(username, StringComparison.InvariantCultureIgnoreCase));
        }
    }

    public PlayerController GetPlayerByIndex(int playerIndex)
    {
        lock (mutex)
        {
            if (playerIndex < 0 || playerIndex >= this.players.Count)
                return null;

            return this.players[playerIndex];
        }
    }

    public IReadOnlyList<PlayerController> GetAssignedPlayers()
    {
        lock (mutex)
        {
            return this.players.Where(x => x.IsAssigned).ToList();
        }
    }

    public IReadOnlyList<PlayerController> GetAliveAndAssignedPlayers()
    {
        lock (mutex)
        {
            return this.players.Where(x => !x.Lynched && x.IsAssigned).ToList();
        }
    }

    public IReadOnlyList<PlayerController> GetAlivePlayers()
    {
        lock (mutex)
        {
            return this.players.Where(x => !x.Lynched).ToList();
        }
    }

    public IReadOnlyList<PlayerController> GetDeadPlayers()
    {
        lock (mutex)
        {
            return this.players.Where(x => x.Lynched).ToList();
        }
    }

    public IReadOnlyList<PlayerController> GetPlayers()
    {
        lock (mutex)
        {
            return this.players.ToList();
        }
    }

}