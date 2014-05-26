using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoverCollisionSensor : MonoBehaviour {
    private List<CoverController> _covers = new List<CoverController>();    // Everything this trigger is colliding with
    private bool _triggered = false;
    
    public List<CoverController> Covers {
        get { return _covers; }
    }
    
    public bool Triggered {
        get { return _triggered; }
    }
    
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.transform.position, 0.05f);
    }

    public void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Cover")) {
            _triggered = true;
            _covers.Add(other.GetComponent<CoverController>());
        }
    }

    /// <summary>
    /// Untriggers the sensor and deactivates the cover it *was* touching.
    /// We can deactivate cover here because both sensors MUST be touching
    /// the cover in order for it to be activated.
    /// </summary>
    public void OnTriggerExit(Collider other) {
        if (other.CompareTag("Cover")) {
            _triggered = false;

            CoverController cover = other.GetComponent<CoverController>();
            cover.Deactivate();
            _covers.Remove(cover);
        }
    }
}
