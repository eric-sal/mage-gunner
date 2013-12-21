using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

/// <summary>
/// Handles all pathfinding logic for NPCs.
/// </summary>
public class PathfinderAI : MonoBehaviour {

    const float NEXT_NODE_DISTANCE = 1f;  //The max distance from the AI to a node for it to continue to the next node
    const int FORWARD = 1;
    const int BACKWARD = -1;

    /* *** Member Variables *** */

    public Waypoint firstWaypoint;  //The first waypoint to calculate the path to
    public Vector3 targetPosition;  //The end-point to move toward

    private BaseCharacterState _character;
    private int _currentNode = 0; //The node in the A-star pathfinding graph we are currently moving toward
    private int _currentWaypoint = 0; //The waypoint object we are currently moving toward
    private int _direction = FORWARD; //A 1 indicates we are moving "forwards" on the route and a -1 indicates "backwards"
    private Path _path;   //The calculated path
    private Seeker _seeker;
    private List<Waypoint> _waypoints;

    /* *** Constructors *** */

    void Awake() {
        _character = GetComponent<BaseCharacterState>();
        _seeker = GetComponent<Seeker>();
        _waypoints = new List<Waypoint>();
    }

    void Start() {

        if (this.firstWaypoint != null) {

            Waypoint next = this.firstWaypoint;
            while (next != null) {
                _waypoints.Add(next);
                next = next.next;
            }

            this.targetPosition = _waypoints[_currentWaypoint].transform.position;
        }

        //Start a new path to the targetPosition, return the result to the OnPathComplete function
        _seeker.StartPath(this.transform.position, this.targetPosition, OnPathComplete);
    }

    protected Waypoint _CurrentWaypoint {
        get {
            return _waypoints[_currentWaypoint];
        }
    }

    protected Waypoint _NextWaypoint {
        get {
            int nextWaypoint = _currentWaypoint + _direction;
            if (nextWaypoint >= _waypoints.Count || nextWaypoint < 0) {
                // change direction
                nextWaypoint = _currentWaypoint + _direction * -1;
            }
            return _waypoints[nextWaypoint];
        }
    }

    /* *** Member Methods *** */

    /// <summary>
    /// Move the NPC along the calculated path.
    /// </summary>
    /// <param name='speed'>
    /// The speed at which to move.
    /// </param>
    public void MoveAlongPath(float speed) {

        if (_path == null) {
            //We have no path to move along yet
            return;
        }

        if (_currentNode >= _path.vectorPath.Count) {
            _UpdateTargetPosition();
            _path = null;
            _seeker.StartPath(this.transform.position, this.targetPosition, OnPathComplete);
            _character.velocity = Vector3.zero;
            return;
        }

        //Direction to the next waypoint
        Vector3 velocity = (_path.vectorPath[_currentNode] - this.transform.position);
        _character.velocity = Vector3.ClampMagnitude(velocity, 1) * speed;

        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        if (Vector3.Distance(this.transform.position, _path.vectorPath[_currentNode]) < NEXT_NODE_DISTANCE) {
            _currentNode++;
            return;
        }
    }

    /// <summary>
    /// Callback sent to the Seeker.StartPath method.
    /// </summary>
    public void OnPathComplete(Path p) {
        if (!p.error) {
            _path = p;
            _currentNode = 0;    //Reset the waypoint counter
        }
        else {
            Debug.Log(p.errorLog);
        }
    }

    /// <summary>
    /// Sets the target position equal to the next waypoint
    /// </summary>
    protected void _UpdateTargetPosition() {

        int nextWaypoint = _waypoints.IndexOf(_NextWaypoint);
        _direction = nextWaypoint - _currentWaypoint;
        _currentWaypoint = nextWaypoint;
        _UpdateTargetPosition(_waypoints[_currentWaypoint].transform.position);
    }

    /// <summary>
    /// Sets the target position to the given target location.
    /// </summary>
    protected void _UpdateTargetPosition(Vector3 target) {
        this.targetPosition = target;
    }
}
