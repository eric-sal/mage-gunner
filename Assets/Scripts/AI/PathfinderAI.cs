using System;
using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

/// <summary>
/// Handles all pathfinding logic for NPCs.
/// </summary>
public class PathfinderAI : MonoBehaviour {

    public delegate void OnEndOfPath(); // The signature of a function to call when the end of the current path is reached

    /* *** Member Variables *** */

    public Vector3 targetPosition;  //The end-point to move toward

    protected BaseCharacterState _character;
    protected int _currentNode = 0; //The node in the A-star pathfinding graph we are currently moving toward
    protected Path _path;   //The calculated path
    protected Rigidbody _rigidbody;
    protected Seeker _seeker;

    /* *** Constructors *** */

    void Awake() {
        _character = GetComponent<BaseCharacterState>();
        _seeker = GetComponent<Seeker>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    /* *** Member Methods *** */

    /// <summary>
    /// Move the NPC along the calculated path.
    /// </summary>
    /// <param name='speed'>
    /// The speed at which to move.
    /// </param>
    public void MoveAlongPath(float speed, OnEndOfPath callback = null) {

        if (_path == null) {
            //We have no path to move along yet
            return;
        }

        if (_currentNode >= _path.vectorPath.Count) {
            //End of path reached
            if (callback != null) {
                callback.Invoke();
            }

            return;
        }

        //Determine how far we will advance down the path during fixedDeltaTime
        Vector3 velocity = Vector3.zero;
        float distanceRemaining = speed * Time.fixedDeltaTime; // the max distance we will move during FixedUpdate
        float distanceToNext = Vector3.Distance(this.transform.position, _path.vectorPath[_currentNode]);
        Vector3 direction = (_path.vectorPath[_currentNode] - this.transform.position).normalized;

        if (distanceToNext > distanceRemaining) {
            //We won't reach the next waypoint in time, just head in that direction
            velocity = Vector3.ClampMagnitude(direction * speed, speed);
        } else {
            //We can reach the next waypoint, so adjust velocity so we don't overshoot the waypoint.
            velocity = direction * distanceToNext / Time.fixedDeltaTime;
            _currentNode++;
        }

        _character.velocity = velocity;
        _rigidbody.velocity = velocity;
    }

    /// <summary>
    /// Callback sent to the Seeker.StartPath method.
    /// </summary>
    public void OnPathCalculated(Path p) {
        
        if (p == null) {
            return;
        }

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
        _seeker.StartPath(this.transform.position, this.targetPosition, OnPathCalculated);
        _character.velocity = Vector3.zero;
    }

    /// <summary>
    /// Calculate paths to all given target positions and select the shortest path
    /// </summary>
    /// <param name="targets">Targets.</param>
    public System.Collections.IEnumerator TakeShortestPath(IList<Vector3> targets) {
        Path shortestPath = null;
        foreach (Vector3 t in targets) {
            Path p = _seeker.StartPath(this.transform.position, t);
            yield return StartCoroutine((p.WaitForPath()));
            
            if (shortestPath == null || p.vectorPath.Count < shortestPath.vectorPath.Count) {
                shortestPath = p;
            }
        }
        OnPathCalculated(shortestPath);
    }
}
