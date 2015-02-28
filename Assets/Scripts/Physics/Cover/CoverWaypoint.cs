using UnityEngine;
using System.Collections;

public class CoverWaypoint : MonoBehaviour {

    public CoverController attachedTo;
    public bool isActive = false;
    public bool isViable = false;
    public GameObject occupiedBy = null; // The current game object occupying this cover point

    protected static int _layerMask;
    protected static PlayerState _playerState = null;

    void Start() {
        if (this.attachedTo == null) {
            throw new MissingReferenceException(this.ToString() + " is not attached to a CoverController");
        }

        if (_playerState == null) {
            _playerState = GameObject.Find("Player").GetComponentInChildren<PlayerState>();
            _layerMask = (1 << LayerMask.NameToLayer("Obstacles")) | (1 << LayerMask.NameToLayer("Players"));
        }
    }
    
    void FixedUpdate() {
        if (!this.isActive) {
            return;
        }
        Vector3 origin = this.transform.position;
        Vector3 direction = _playerState.aimPoint - origin;
        var hitInfo = new RaycastHit();
        float distance = (direction - origin).sqrMagnitude;
        Physics.Raycast(origin, direction, out hitInfo, distance, _layerMask);
        this.isViable = (hitInfo.collider == null) || (hitInfo.collider.gameObject != _playerState.gameObject);
        Debug.DrawRay(origin, direction, this.isViable ? Color.green : Color.red);
    }

    public void OnTriggerEnter(Collider other) {
        switch (other.name) {

        case "ActiveArea":
            this.isActive = true;
            SceneController.activeCoverWaypoints.Add(this);
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
            SceneController.activeCoverWaypoints.Remove(this);
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
