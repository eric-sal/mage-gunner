using UnityEngine;
using System.Collections;

/// <summary>
/// NPC controller.
/// </summary>
public class NpcController : BaseCharacterController {

    // When checking to see if we can see the player, we want the ray to ignore projectiles.
    private static int _layerMask = ((1 << LayerMask.NameToLayer("Players")) |
        (1 << LayerMask.NameToLayer("Obstacles")) |
        (1 << LayerMask.NameToLayer("Enemies")));

    /* *** Member Variables *** */

    protected Vector2 _estimatedPlayerVelocity;
    protected INpcBehavior _currentBehavior;
    protected PathfinderAI _pathfinderAI;
    protected NpcState _myState;
    protected PlayerState _playerState;
    protected Vector2 _previousPlayerPosition = Vector2.zero;

    public Vector2 estimatedPlayerVelocity {
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

    public override void Awake() {
        base.Awake();

        _currentBehavior = new IdleBehavior();
        _myState = (NpcState)_character;
        _pathfinderAI = GetComponent<PathfinderAI>();
        _playerState = GameObject.Find("Player").GetComponentInChildren<PlayerState>();
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
            Debug.DrawRay(this.transform.position, direction * _myState.sightDistance);

            hitInfo = Physics2D.Raycast(this.transform.position, direction, _myState.sightDistance, _layerMask);
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
            Vector2 currentPlayerPosition = (Vector2)_playerState.transform.position;

            if (_previousPlayerPosition != Vector2.zero) {
                Vector2 deltaPosition = currentPlayerPosition - _previousPlayerPosition;
                _estimatedPlayerVelocity = Vector2.ClampMagnitude(deltaPosition / Time.deltaTime, 1);
            }

            _previousPlayerPosition = currentPlayerPosition;
        } else {
            _previousPlayerPosition = Vector2.zero;
            _estimatedPlayerVelocity = Vector2.zero;
        }
    }
}
