using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoverCollisionSensor : MonoBehaviour {

    /* *** Member Variables *** */

    private List<CoverController> _covers = new List<CoverController>();    // Everything this trigger is colliding with
    private bool _triggered = false;

    /* *** Properties *** */

    public List<CoverController> Covers {
        get { return _covers; }
    }
    
    public bool Triggered {
        get { return _triggered; }
    }

    /* *** Constructors *** */
    public void Awake() {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    /* *** MonoBehavior Methods *** */
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
            CoverController cover = other.GetComponent<CoverController>();
            cover.Deactivate();
            _covers.Remove(cover);

            // We should only deactivate if _covers is empty. If it's not,
            // we're still touching at least one cover.
            if (_covers.Count == 0) {
                _triggered = false;
            }
        }
    }
}
