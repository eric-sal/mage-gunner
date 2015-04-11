using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles all pathfinding logic for NPCs.
/// </summary>
public class PathfinderAI : MonoBehaviour {

    protected const float STOPPING_DISTANCE = 0.01f;

    /* *** Member Variables *** */

    protected NavMeshAgent _agent;

    /* *** Constructors *** */

    void Awake() {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
    }

    /* *** Member Properties *** */

    public bool hasReachedEndOfPath {
        get {
            return _agent.remainingDistance <= STOPPING_DISTANCE;
        }
    }

    /* *** Member Methods *** */

    /// <summary>
    /// Calculate paths to all given target positions and select the shortest path
    /// </summary>
    /// <param name="targets">Possible target positions</param>
    public Vector3 TakeShortestPath(IList<Vector3> targets) {

        if (targets.Count == 0) {
            throw new ArgumentException("targets cannot be empty");
        }

        var sourcePosition = this.transform.position;
        NavMeshPath shortestPath = null;
        float shortestDistance = 0f;
        Vector3 selectedTarget = targets[0];

        foreach (Vector3 t in targets) {
            var p = new NavMeshPath();
            NavMesh.CalculatePath(sourcePosition, t, NavMesh.AllAreas, p);

            if (shortestPath == null || _IsPathShorter(p, shortestDistance)) {
                shortestPath = p;
                shortestDistance = _PathLength(shortestPath);
                selectedTarget = t;
            }
        }

        _agent.SetPath(shortestPath);
        return selectedTarget;
    }

    public void ResetPath() {
        _agent.ResetPath();
    }
    
    protected static float _PathLength(NavMeshPath path) {

        if (path.corners.Length < 2) {
            return 0f;
        }

        Vector3 previousCorner = path.corners[0];
        float lengthSoFar = 0.0f;
        for (int i = 1; i < path.corners.Length; i++) {
            Vector3 currentCorner = path.corners[i];
            lengthSoFar += Vector3.Distance(previousCorner, currentCorner);
            previousCorner = currentCorner;
		}
        return lengthSoFar;
    }

    protected static bool _IsPathShorter(NavMeshPath path, float distance) {
        return _PathLength(path) < distance;
    }

}
