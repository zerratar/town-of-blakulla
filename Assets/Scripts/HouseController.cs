using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseController : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject WP_Close;
    public GameObject WP_Far;
    public int WaypointCount = 2;

    void Start()
    {
        if (WP_Close && WP_Far)
        {
            return;
        }
        
        var wp = this.transform.Find("Waypoints");
        if (wp && wp.childCount == WaypointCount)
        {
            this.WP_Close = wp.GetChild(0).gameObject;
            this.WP_Far = wp.GetChild(1).gameObject;
            return;
        }

        Debug.LogError($"Waypoints not found for house: {this.name}");
    }

    public Vector3 GetWaypoint(int number)
    {
        if (!WP_Close || !WP_Far) return Vector3.zero;
        if (number == 0) return this.WP_Close.transform.position;
        else return this.WP_Far.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
