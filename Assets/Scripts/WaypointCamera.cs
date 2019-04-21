using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointCamera : MonoBehaviour
{

    public GameObject[] Waypoints;

    public Transform ExecutionPosition;

    public bool moveToNextWaypoint = false;

    public float moveTimer = 0f;
    public float moveTime = 0f;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool showExecution;

    public void MoveToNextWaypoint(float time)
    {
        //Debug.Log($"Camera->MoveToNextWaypoint({time});");
        moveToNextWaypoint = true;
        moveTime = time;
        moveTimer = time;
        this.startPosition = this.transform.position;
        this.startRotation = this.transform.rotation;
        this.CurrentWaypointIndex = (this.CurrentWaypointIndex + 1) % this.Waypoints.Length;
    }

    public int CurrentWaypointIndex { get; private set; } = -1;

    public void BeginExecution()
    {
        this.showExecution = true;
        this.moveTime = 0.5f;
        this.moveTimer = 0.5f;
    }

    public void EndExecution()
    {
        showExecution = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveTimer <= 0) return;

        moveTimer -= Time.deltaTime;

        var target = GetTargetTransform();
        var targetPos = target.position;
        var targetRot = target.rotation;

        if (moveTimer <= 0)
        {
            this.transform.position = targetPos;
            this.transform.rotation = targetRot;
            this.ArrivedAtWaypoint();
            return;
        }

        this.WaypointProgress = 1f - (moveTimer / moveTime);
        this.transform.position = Vector3.Lerp(
            this.startPosition, targetPos, WaypointProgress);

        this.transform.rotation = Quaternion.Lerp(
            this.startRotation, targetRot, WaypointProgress);
    }

    public float WaypointProgress { get; private set; }


    private Transform GetTargetTransform()
    {
        if (showExecution)
        {
            return this.ExecutionPosition;
        }

        return this.Waypoints[CurrentWaypointIndex].transform;
    }

    private void ArrivedAtWaypoint()
    {
        Debug.Log("Camera in position");
        moveTimer = 0f;
        moveTime = 0f;
        moveToNextWaypoint = false;
    }
}
