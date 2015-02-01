using UnityEngine;
using System.Collections;

public class CoverWaypoint : MonoBehaviour {

    public CoverController attachedTo;
    public bool isActive = false;
    public bool isViable = false;
    public GameObject occupiedBy = null; // The current game object occupying this cover point

    protected static PlayerState _playerState = null;


	void Start() {
	    if (this.attachedTo == null) {
            throw new MissingReferenceException(this.ToString() + " is not attached to a CoverController");
        }

        if (_playerState == null) {
            _playerState = GameObject.Find("Player").GetComponentInChildren<PlayerState>();
        }
	}
	
	void FixedUpdate() {
        if (!this.isActive) {
            return;
        }

        Vector3 origin = this.transform.position;
        Vector3 direction = _playerState.aimPoint - origin;
        Debug.DrawRay(origin, direction, Color.yellow);
        var hitInfo = new RaycastHit();
        Physics.Raycast(origin, direction, out hitInfo);
        this.isViable = hitInfo.collider.gameObject != _playerState.gameObject;
	}

    public void OnTriggerEnter(Collider other) {
        switch (other.name) {

        case "ActiveArea":
            this.isActive = true;
            break;

        case "VisibleArea":
        case "Bullet":
        case "Floor":
            break;

        default:
            this.occupiedBy = other.gameObject;
            break;
        }
    }
    
    public void OnTriggerExit(Collider other) {
        switch (other.name) {
            
        case "ActiveArea":
            this.isActive = false;
            break;
            
        case "VisibleArea":
        case "Bullet":
        case "Floor":
            break;
            
        default:
            this.occupiedBy = null;
            break;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = this.isViable ? Color.green : Color.red;
        Gizmos.DrawSphere(this.transform.position, 0.2f);
    }
}
