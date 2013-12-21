using UnityEngine;
using System.Collections;

/// <summary>
/// NPC controller.
/// </summary>
public class NpcController : BaseCharacterController {

    /* *** Member Variables *** */

    protected Vector3 _estimatedPlayerVelocity;
    protected INpcBehavior _currentBehavior;
    protected PathfinderAI _pathfinderAI;
    protected int _layerMask;
    protected NpcState _myState;
    protected PlayerState _playerState;
    protected Vector3 _previousPlayerPosition = Vector3.zero;

    public Vector3 estimatedPlayerVelocity {
        get { return _estimatedPlayerVelocity; }
    }

    public NpcState myState {
        get { return _myState; }
    }

    public PathfinderAI pathfinderAI {
        get { return _pathfinderAI; }
    }

    public PlayerState playerState {
        get { return _playerState; }
    }

    /* *** Constructors *** */

    void Start() {
        _currentBehavior = new IdleBehavior();
        _myState = (NpcState)_character;
        _pathfinderAI = GetComponent<PathfinderAI>();
        _playerState = GameObject.Find("Player").GetComponentInChildren<PlayerState>();

        // When checking to see if we can see the player, we want the ray to ignore projectiles.
        _layerMask = ((1 << LayerMask.NameToLayer("Players")) |
                      (1 << LayerMask.NameToLayer("Obstacles")) |
                      (1 << LayerMask.NameToLayer("Enemies")));
    }

    /* *** MonoBehaviour Methods *** */

    /// <summary>
    /// Simulated player input for aiming and firing.
    /// </summary>
    public override void Update() {
        base.Update();

        if (_myState.canSeePlayer) {
            _currentBehavior = new AttackBehavior(this);
        } else {
            //_currentBehavior = new IdleBehavior();
            _currentBehavior = new PatrolBehavior(this);
        }

        _currentBehavior.doUpdate();
    }

    /// <summary>
    /// Move the NPC.
    /// Perform additional functionality that should happen at fixed
    /// intervals in FixedUpdate().
    /// </summary>
    public override void FixedUpdate() {
        _FindPlayer();
        _EstimatePlayerVelocity();

        _currentBehavior.doFixedUpdate();
        base.FixedUpdate();
    }

    /* *** Member Methods *** */

    /// <summary>
    /// Try to find the player in the NPC's field of vision.
    /// </summary>
    protected void _FindPlayer() {
        // Since the raycast starts inside our enemy, we want to ignore ourself when casting the ray to find the player.
        LayerMask myLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        _myState.canSeePlayer = false;
        //_pathfinderAI.targetPosition = _playerState.transform.position;

        RaycastHit2D hitInfo;
        Quaternion quato = Quaternion.LookRotation(_myState.lookDirection, Vector3.forward);

        for (int i = _myState.fieldOfVision / 2 * -1; i < _myState.fieldOfVision / 2; i++) {
            Vector3 direction = quato * Quaternion.Euler(0, i, 0) * Vector3.forward;
            Vector2 direction2D = new Vector2(direction.x, direction.y);
            Debug.DrawRay(this.transform.position, direction * _myState.sightDistance);

            Vector2 position2D = new Vector2(this.transform.position.x, this.transform.position.y);
            hitInfo = Physics2D.Raycast(position2D, direction2D, _myState.sightDistance, _layerMask);

            if (hitInfo.collider == _playerState.gameObject.collider2D) {
                _myState.canSeePlayer = true;
                break;
            }
        }

        gameObject.layer = myLayer;
    }

    /// <summary>
    /// Estimates the player's movement velocity.
    /// Used in anticipating the player's movement when aiming.
    /// </summary>
    protected void _EstimatePlayerVelocity() {
        if (_myState.anticipatePlayerMovement && _myState.canSeePlayer) {
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
