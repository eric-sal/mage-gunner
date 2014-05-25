using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoverCollisionSensor : MonoBehaviour {
    private List<Collider> _others = new List<Collider>();    // Everything this trigger is colliding with
    private bool _triggered = false;
    
    public List<Collider> Others {
        get { return _others; }
    }
    
    public bool Triggered {
        get { return _triggered; }
    }
    
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.transform.position, 0.1f);
    }

    public void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            return;
        }
        
        _triggered = true;
        _others.Add(other);
    }

    public void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            return;
        }

        _triggered = false;
        _others.Remove(other);
    }
}
