using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A waypoint in a patrol route
/// </summary>
public class Waypoint : MonoBehaviour {

    public Vector2 lookDirection;   // When the NPC reaches this waypoint, in which direction should they look?
    public Waypoint next; // the next waypoint (if any) in a path

    private static GameObject _waypointPrefab;

    public Vector2 lookPosition {
        get { return (Vector2)this.transform.position + lookDirection; }
    }

    /// <summary>
    /// Create a Waypoint instance in the scene.
    /// </summary>
    public static Waypoint Create(Vector2 spawnPosition) {
        if (_waypointPrefab == null) {
            _waypointPrefab = (GameObject)Resources.Load("Prefabs/Waypoint");
        }

        GameObject waypointInstance = (GameObject)Instantiate(_waypointPrefab, spawnPosition, _waypointPrefab.transform.rotation);
        return waypointInstance.GetComponent<Waypoint>();
    }

    /// <summary>
    /// Ensure that the waypoint is visible in the Unity editor window.
    /// </summary>
    void OnDrawGizmos() {
        Vector2 position = (Vector2)this.transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(position, 0.2f);

        // Draw a line representing this WP's lookDirection
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(position, lookDirection.normalized + position);

        // Draw a line linking this WP to the next WP
        if (this.next != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(position, this.next.transform.position);
        }
    }

    public override string ToString() {
        return string.Format("{0} : @ {1}", this.name, this.transform.position);
    }
}
