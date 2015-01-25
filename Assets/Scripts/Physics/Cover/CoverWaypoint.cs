using UnityEngine;
using System.Collections;

public class CoverWaypoint : MonoBehaviour {

    public CoverController attachedTo;

    protected bool _isViable = false;
    protected GameObject _occupiedBy = null;

    public bool isViable {
        get { return _isViable; }
    }

    protected static PlayerState _playerState = null;

    /// <summary>
    /// The current game object occupying this cover point
    /// </summary>
    public GameObject occupiedBy {
        get { return _occupiedBy; }
    }

	void Start() {
	    if (this.attachedTo == null) {
            throw new MissingReferenceException(this.ToString() + " is not attached to a CoverController");
        }

        if (_playerState == null) {
            _playerState = GameObject.Find("Player").GetComponentInChildren<PlayerState>();
        }
	}
	
	void FixedUpdate() {
        Vector3 origin = this.transform.position;
        Vector3 direction = _playerState.aimPoint - origin;
        Debug.DrawRay(origin, direction, Color.yellow);
        var hitInfo = new RaycastHit();
        Physics.Raycast(origin, direction, out hitInfo);
        _isViable = hitInfo.collider.gameObject != _playerState.gameObject;
	}

    public void OnTriggerEnter(Collider other) {
        _occupiedBy = other.gameObject;
    }
    
    public void OnTriggerExit(Collider other) {
        _occupiedBy = null;
    }

    void OnDrawGizmos() {
        Gizmos.color = _isViable ? Color.green : Color.red;
        Gizmos.DrawSphere(this.transform.position, 0.2f);
    }
}
