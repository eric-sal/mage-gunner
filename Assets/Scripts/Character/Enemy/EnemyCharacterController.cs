using UnityEngine;
using System.Collections;

public class EnemyCharacterController : BaseCharacterController {

    protected PathfinderAI _npcAI;
    protected bool _canSeePlayer;
    protected EnemyState _myState;
    protected int _layerMask;
    protected PlayerState _playerState;
    protected Vector3 _previousPlayerPosition = Vector3.zero;
    protected Vector3 _estimatedPlayerVelocity;

    public void Start() {
        _npcAI = GetComponent<PathfinderAI>();
        _myState = (EnemyState)_character;
        _playerState = GameObject.Find("Player").GetComponentInChildren<PlayerState>();

        // When checking to see if we can see the player, we want the ray to ignore projectiles.
        _layerMask = ((1 << LayerMask.NameToLayer("Players"))
                    | (1 << LayerMask.NameToLayer("Obstacles"))
                    | (1 << LayerMask.NameToLayer("Enemies")));
    }

    protected override void CaptureInput() {
        FindPlayer();
        EstimatePlayerVelocity();

        if (!_canSeePlayer || _playerState.health <= 0) {
            return;
        }

        Aim();


        var gun = this.equippedFirearm;
        if (gun != null) {
            if (gun.IsEmpty) {
                gun.Reload();
            } else {
                Fire();
            }
        }
    }

    protected override void Act() {
        if (_npcAI.path == null) {
            //We have no path to move after yet
            return;
        }

        if (_npcAI.currentWaypoint >= _npcAI.path.vectorPath.Count) {
            Debug.Log("End Of Path Reached");
            return;
        }

        //Direction to the next waypoint
        Vector3 velocity = (_npcAI.path.vectorPath[_npcAI.currentWaypoint] - transform.position);
        _myState.velocity = Vector3.ClampMagnitude(velocity, 1) * _myState.maxWalkSpeed;
        _moveable.velocity = new Vector3(_myState.velocity.x, _myState.velocity.y);

        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        if (Vector3.Distance(transform.position, _npcAI.path.vectorPath[_npcAI.currentWaypoint]) < _npcAI.nextWaypointDistance) {
            _npcAI.currentWaypoint++;
            return;
        }
    }

    protected override void Aim() {
        Vector3 playerPosition = _playerState.transform.position + _estimatedPlayerVelocity;
        _reticle.LerpTo(playerPosition, _myState.lookSpeed);
    }

    protected void FindPlayer() {
        _canSeePlayer = false;
        _npcAI.targetPosition = _playerState.transform.position;

        RaycastHit hitInfo;
        Vector3 direction = _playerState.transform.position - _myState.transform.position;

        Debug.DrawRay(_myState.transform.position, direction);

        if (Physics.Raycast(_myState.transform.position, direction, out hitInfo, 20, _layerMask)) {
            if (hitInfo.collider == _playerState.gameObject.collider) {
                _canSeePlayer = true;
            }
        }
    }

    protected void EstimatePlayerVelocity() {
        if (_myState.anticipatePlayerMovement && _canSeePlayer) {
            Vector3 currentPlayerPosition = _playerState.transform.position;

            if (_previousPlayerPosition != Vector3.zero) {
                Vector3 deltaPosition = currentPlayerPosition - _previousPlayerPosition;
                _estimatedPlayerVelocity = Vector3.ClampMagnitude(deltaPosition / Time.deltaTime, 1);
            }

            _previousPlayerPosition = currentPlayerPosition;
        } else {
            _previousPlayerPosition = Vector3.zero;
            _estimatedPlayerVelocity = Vector3.zero;
        }
    }
}
