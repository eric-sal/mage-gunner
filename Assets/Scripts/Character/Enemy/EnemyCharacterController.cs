using UnityEngine;
using System.Collections;

/// <summary>
/// Enemy character controller.
/// </summary>
public class EnemyCharacterController : BaseCharacterController {

    /* *** Member Variables *** */

    protected bool _canSeePlayer;
    protected Vector3 _estimatedPlayerVelocity;
    protected PathfinderAI _pathfinderAI;
    protected int _layerMask;
    protected EnemyState _myState;
    protected PlayerState _playerState;
    protected Vector3 _previousPlayerPosition = Vector3.zero;

    /* *** Constructors *** */

    public void Start() {
        _myState = (EnemyState)_character;
        _pathfinderAI = GetComponent<PathfinderAI>();
        _playerState = GameObject.Find("Player").GetComponentInChildren<PlayerState>();

        // When checking to see if we can see the player, we want the ray to ignore projectiles.
        _layerMask = ((1 << LayerMask.NameToLayer("Players")) |
                      (1 << LayerMask.NameToLayer("Obstacles")) |
                      (1 << LayerMask.NameToLayer("Enemies")));
    }

    /* *** Protected Methods *** */

    /// <summary>
    /// Simulated player input.
    /// Called from Update in BaseCharacterController.
    /// </summary>
    protected override void CaptureInput() {
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

    /// <summary>
    /// Modify the NPC's movement.
    /// Perform additional functionality that should happen at fixed
    /// intervals in FixedUpdate().
    /// </summary>
    protected override void Act() {
        FindPlayer();
        EstimatePlayerVelocity();

        _pathfinderAI.MoveAlongPath(_myState.maxWalkSpeed);
    }

    /// <summary>
    /// Simulated reticle aiming.
    /// </summary>
    protected override void Aim() {
        Vector3 playerPosition = _playerState.transform.position + _estimatedPlayerVelocity;
        _reticle.LerpTo(playerPosition, _myState.lookSpeed);
    }

    /// <summary>
    /// Try to find the player in the NPC's field of vision.
    /// </summary>
    protected void FindPlayer() {
        _canSeePlayer = false;
        _pathfinderAI.targetPosition = _playerState.transform.position;

        RaycastHit hitInfo;
        Vector3 direction = _playerState.transform.position - _myState.transform.position;

        if (Physics.Raycast(_myState.transform.position, direction, out hitInfo, 20, _layerMask)) {
            if (hitInfo.collider == _playerState.gameObject.collider) {
                _canSeePlayer = true;
            }
        }
    }

    /// <summary>
    /// Estimates the player's movement velocity.
    /// Used in anticipating the player's movement when aiming.
    /// </summary>
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
