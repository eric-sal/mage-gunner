using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A waypoint in a patrol route
/// </summary>
public class Waypoint : MonoBehaviour {

    public int index; // position of waypoint in a patrol route

    /// <summary>
    /// Ensure that the waypoint is visible in the Unity editor window.
    /// </summary>
    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(this.transform.position, 0.2f);
    }

    public override string ToString() {
        return string.Format("{0} : index {1} @ {2}", this.name, this.index, this.transform.position);
    }

    /// <summary>
    /// Helper class for comparing the index of two waypoints.
    /// </summary>
    public class Comparer : IComparer<Waypoint> {
        public int Compare(Waypoint wp1, Waypoint wp2) {
            if (wp1.index == wp2.index) {
                return 0;
            } else {
                return wp1.index < wp2.index ? -1 : 1;
            }
        }
    }
}
