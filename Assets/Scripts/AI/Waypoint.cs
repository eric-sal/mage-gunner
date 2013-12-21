using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A waypoint in a patrol route
/// </summary>
public class Waypoint : MonoBehaviour {

    public Waypoint next; // the next waypoint (if any) in a path

    /// <summary>
    /// Ensure that the waypoint is visible in the Unity editor window.
    /// </summary>
    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(this.transform.position, 0.2f);

        if (this.next != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.transform.position, this.next.transform.position);
        }
    }

    public override string ToString() {
        return string.Format("{0} : @ {2}", this.name, this.transform.position);
    }
}
