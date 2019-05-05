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

            foreach (var player in players)
            {
                player.AssignRole(roleManager.GetRandomRole());

                if (UnityEngine.Random.value >= 0.5)
                    player.UpdateLastWill("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam imperdiet commodo urna. Duis eu lectus est. Mauris rhoncus justo turpis, a maximus nisl placerat vitae. Nulla facilisi. Mauris consectetur varius ligula, vitae fringilla turpis scelerisque nec. Duis massa diam, convallis nec odio sed, porta malesuada arcu. Nulla eget eros facilisis neque viverra auctor quis eget metus.\r\n\r\nPellentesque imperdiet eros a porta finibus. Phasellus ut nunc.");
            }
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
            //var role = roleManager.GetRandomRole();
            return this.players[index].Assign(username, color);
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

    public PlayerController FindPlayer(string playerName)
    {
        lock (mutex)
        {
            return players.FirstOrDefault(x =>
                x.PlayerName != null &&
                x.PlayerName.Equals(playerName, StringComparison.InvariantCultureIgnoreCase));
        }
    }

    public int FindPlayerIndex(string username)
    {
        lock (mutex)
        {
            return players.FindIndex(x =>
                x.PlayerName != null &&
                x.PlayerName.Equals(username, StringComparison.InvariantCultureIgnoreCase));
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
            return this.players.Where(x => !x.Dead && x.IsAssigned).ToList();
        }
    }

    public IReadOnlyList<PlayerController> GetAlivePlayers()
    {
        lock (mutex)
        {
            return this.players.Where(x => !x.Dead).ToList();
        }
    }

    public IReadOnlyList<PlayerController> GetDeadPlayers()
    {
        lock (mutex)
        {
            return this.players.Where(x => x.Dead).ToList();
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