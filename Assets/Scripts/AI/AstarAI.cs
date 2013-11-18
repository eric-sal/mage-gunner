using UnityEngine;
using System.Collections;

//Note this line, if it is left out, the script won't know that the class 'Path' exists and it will throw compiler errors
//This line should always be present at the top of scripts which use pathfinding
using Pathfinding;

public class AstarAI : MonoBehaviour {
    //The point to move to
    public Vector3 targetPosition;
    private Seeker _seeker;
    private EnemyCharacterController _controller;
 
    //The calculated path
    public Path path;

    //The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 0.1f;

    //The waypoint we are currently moving towards
    public int currentWaypoint = 0;
 
    public void Start() {
        _seeker = GetComponent<Seeker>();
        _controller = GetComponent<EnemyCharacterController>();
        
        //Start a new path to the targetPosition, return the result to the OnPathComplete function
        _seeker.StartPath(transform.position, targetPosition, OnPathComplete);
    }
    
    public void OnPathComplete(Path p) {
        Debug.Log("Yey, we got a path back. Did it have an error? " + p.error);
        if (!p.error) {
            path = p;
            //Reset the waypoint counter
            currentWaypoint = 0;
        }
    }
}
