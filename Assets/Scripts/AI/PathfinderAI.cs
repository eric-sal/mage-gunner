using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

/// <summary>
/// Handles all pathfinding logic for NPCs.
/// </summary>
public class PathfinderAI : MonoBehaviour {

    const int FORWARD = 1;
    const int BACKWARD = -1;

    /* *** Member Variables *** */

    public Waypoint firstWaypoint;  //The first waypoint to calculate the path to
    public Vector2 targetPosition;  //The end-point to move toward

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
        RestartPath();
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

        //Determine how far we will advanced down the path during fixedDeltaTime
        float distanceRemaining = speed * Time.fixedDeltaTime; // the max distance we will move during FixedUpdate
        Vector3 position = this.transform.position;

        while (distanceRemaining > 0f && _currentNode < _path.vectorPath.Count) {

            float distanceToNext = Vector2.Distance(position, _path.vectorPath[_currentNode]);

            if (distanceToNext > distanceRemaining) {
                //We won't reach the next waypoint in time, just head in that direction
                Vector3 direction = (_path.vectorPath[_currentNode] - position).normalized;
                position += direction * distanceRemaining;

                //Set the velocity, but we've already overridden the movement so it's just for facing purposes
                _character.velocity = Vector2.ClampMagnitude(direction * speed, speed);

            } else {
                //We can reach the next waypoint, how much further can we go?
                position = _path.vectorPath[_currentNode];
                _currentNode++;
            }

            distanceRemaining -= distanceToNext;
        }

        this.transform.position = position;

        if (_currentNode >= _path.vectorPath.Count) {
            //End of path reached
            _UpdateTargetPosition();
            RestartPath();
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
        } else {
            Debug.Log(p.errorLog);
        }
    }

    /// <summary>
    /// Discard the current path, if any, and create a new path to the current targetPosition
    /// </summary>
    public void RestartPath() {
        _path = null;
        _seeker.StartPath(this.transform.position, this.targetPosition, OnPathComplete);
        _character.velocity = Vector2.zero;
    }

    /// <summary>
    /// Sets the target position equal to the next waypoint
    /// </summary>
    protected void _UpdateTargetPosition() {

        if (_waypoints.Count == 0) {
            return;
        }

        int nextWaypoint = _waypoints.IndexOf(_NextWaypoint);
        _direction = nextWaypoint - _currentWaypoint;
        _currentWaypoint = nextWaypoint;
        _UpdateTargetPosition(_waypoints[_currentWaypoint].transform.position);
    }

    /// <summary>
    /// Sets the target position to the given target location.
    /// </summary>
    protected void _UpdateTargetPosition(Vector2 target) {
        this.targetPosition = target;
    }
}
