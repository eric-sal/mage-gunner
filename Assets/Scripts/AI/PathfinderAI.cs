﻿using UnityEngine;
using System.Collections;
using Pathfinding;

/// <summary>
/// Handles all pathfinding logic for NPCs.
/// </summary>
public class PathfinderAI : MonoBehaviour {

    const float NEXT_WAYPOINT_DISTANCE = 0.1f;  //The max distance from the AI to a waypoint for it to continue to the next waypoint

    /* *** Member Variables *** */

    public Vector3 targetPosition;  //The end-point to move toward

    private BaseCharacterState _character;
    private int _currentWaypoint = 0; //The waypoint we are currently moving toward
    private MoveableObject _moveable;
    private Path _path;   //The calculated path
    private Seeker _seeker;

    /* *** Constructors *** */

    public void Start() {
        _character = GetComponent<BaseCharacterState>();
        _moveable = GetComponent<MoveableObject>();
        _seeker = GetComponent<Seeker>();
        
        //Start a new path to the targetPosition, return the result to the OnPathComplete function
        _path = _seeker.StartPath(this.transform.position, this.targetPosition, OnPathComplete);
    }

    /* *** Public Methods *** */

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

        if (_currentWaypoint >= _path.vectorPath.Count) {
            // Debug.Log("End Of Path Reached");
            _character.velocity = Vector3.zero;
            _moveable.velocity = Vector3.zero;
            return;
        }

        //Direction to the next waypoint
        Vector3 velocity = (_path.vectorPath[_currentWaypoint] - this.transform.position);
        _character.velocity = Vector3.ClampMagnitude(velocity, 1) * speed;
        _moveable.velocity = new Vector3(_character.velocity.x, _character.velocity.y);

        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        if (Vector3.Distance(this.transform.position, _path.vectorPath[_currentWaypoint]) < NEXT_WAYPOINT_DISTANCE) {
            _currentWaypoint++;
            return;
        }
    }

    /// <summary>
    /// Callback sent to the Seeker.StartPath method.
    /// </summary>
    public void OnPathComplete(Path p) {
        // Debug.Log("Yey, we got a path back. Did it have an error? " + p.error);
        if (!p.error) {
            _path = p;
            _currentWaypoint = 0;    //Reset the waypoint counter
        }
    }
}