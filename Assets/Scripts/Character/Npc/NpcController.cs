using UnityEngine;
using System.Collections;

/// <summary>
/// NPC controller.
/// </summary>
public class NpcController : BaseCharacterController {

    /* *** Member Variables *** */

    protected Vector2 _estimatedPlayerVelocity;
    protected INpcBehavior _currentBehavior;
    protected PathfinderAI _pathfinderAI;
    protected int _layerMask;
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
        // When checking to see if we can see the player, we want the ray to ignore projectiles.
        _layerMask = ((1 << LayerMask.NameToLayer("Players")) |
                      (1 << LayerMask.NameToLayer("Obstacles")) |
                      (1 << LayerMask.NameToLayer("Enemies")));
        _myState = (NpcState)_character;
        _pathfinderAI = GetComponent<PathfinderAI>();
        _myState.startingPosition = _pathfinderAI.targetPosition = this.transform.position;
        _playerState = GameObject.Find("Player").GetComponentInChildren<PlayerState>();
        _currentBehavior = new IdleBehavior(this);
    }

    /* *** MonoBehaviour Methods *** */

    /// <summary>
    /// Simulated player input for aiming and firing.
    /// </summary>
    public override void Update() {
        base.Update();

        _currentBehavior = _currentBehavior.GetNextBehavior();

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

        if (_currentBehavior as PatrolBehavior == null) {
            //PatrolBehavior handles movement on its own
            //TODO: Behavior should indicate to FixedUpdate whether
            //or not do its thing or maybe have a BaseBehavior that
            //does what this function does now?
            this.rigidbody2D.velocity = _character.velocity;
        }

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

        bool canSeePlayer = false;

        RaycastHit2D hitInfo;
        Quaternion quato = Quaternion.LookRotation(_myState.lookDirection, Vector3.forward);

        for (int i = _myState.fieldOfVision / 2 * -1; i < _myState.fieldOfVision / 2; i++) {
            Vector3 direction = quato * Quaternion.Euler(0, i, 0) * Vector3.forward;
            //Debug.DrawRay(this.transform.position, direction * _myState.sightDistance);

            hitInfo = Physics2D.Raycast(this.transform.position, direction, _myState.sightDistance, _layerMask);
            if (hitInfo.collider != null && Object.ReferenceEquals(hitInfo.collider.gameObject, _playerState.gameObject)) {
                canSeePlayer = true;
                _myState.playerPosition = _playerState.transform.position;
                break;
            }
        }

        if (canSeePlayer) {
            _myState.didSeePlayer = false;
        } else if (canSeePlayer != _myState.canSeePlayer) {
            _myState.didSeePlayer = _myState.canSeePlayer && !canSeePlayer;
        }
        _myState.canSeePlayer = canSeePlayer;

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
