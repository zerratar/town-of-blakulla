using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DummyController : MonoBehaviour
{
    public Transform GallowPoint;

    public NavMeshAgent agent;

    private NavMeshPath currentPath;


    // Start is called before the first frame update
    void Start()
    {
        if (!agent) this.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent) this.GetComponent<NavMeshAgent>();
        if (!agent)
        {
            Debug.LogError("No agent, why tho?");
            return;
        }

        agent.SetDestination(GallowPoint.position);
    }
}
