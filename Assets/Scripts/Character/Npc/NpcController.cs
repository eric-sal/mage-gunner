using UnityEngine;
using System.Collections;

/// <summary>
/// NPC controller.
/// </summary>
public class NpcController : BaseCharacterController {

    /* *** Member Variables *** */

    protected AttackBehavior _attackBehavior;
    protected BaseBehavior[] _behaviors;
    protected ChaseBehavior _chaseBehavior;
    protected Vector2 _estimatedPlayerVelocity;
    protected IdleBehavior _idleBehavior;
    protected PathfinderAI _pathfinderAI;
    protected int _layerMask;
    protected NpcState _myState;
    protected PatrolBehavior _patrolBehavior;
    protected PlayerState _playerState;
    protected Vector2 _previousPlayerPosition = Vector2.zero;

    public BaseBehavior[] behaviors {
        get { return _behaviors; }
    }

    public Vector2 estimatedPlayerVelocity {
        get { return _estimatedPlayerVelocity; }
    }

    // The expectedPlayerPosition is where the NPC expects the player to be based on
    // the last known player position, the estimated velocity, and the amount of time
    // that has passed since they last saw the player.
    public Vector2 expectedPlayerPosition {
        get {
            Vector2 expected;
            Vector2 estimatedDistanceTraveled = estimatedPlayerVelocity * _myState.timeSinceDidSeePlayer;

            // Since the NPC doesn't have any spatial reasoning, we'll fake it for them
            // to hopefully make the NPCs *look* smart.
            // Cast a ray from the last known player position to our expected player
            // position. If we hit an obstacle, then we know that the player couldn't
            // have moved through it. This may provide more info to the NPC than they
            // would/should normally have, but the intention is to create the illusion
            // of intelligence, so we'll live with it.
            RaycastHit2D hitInfo = Physics2D.Raycast(_myState.playerPosition, estimatedDistanceTraveled, estimatedDistanceTraveled.magnitude);
            if (hitInfo.collider != null) {
                expected = hitInfo.point;
            } else {
                expected = _myState.playerPosition + estimatedDistanceTraveled;
            }

            return expected;
        }
    }

    public NpcState myState {
        get { return _myState; }
    }

    public PathfinderAI pathfinderAI {
        get { return _pathfinderAI; }
    }

    public PatrolBehavior patrolBehavior {
        get { return _patrolBehavior; }
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
        if (_myState.startingPosition == null) {
            // startingPosition should be a Waypoint. If it is null, we'll create a new Waypoint for this NPC on the fly.
            _myState.startingPosition = Waypoint.Create(this.transform.position);
            _myState.startingPosition.lookDirection = _myState.lookDirection;
        }
        _pathfinderAI.targetPosition = _myState.startingPosition.transform.position;
        _playerState = GameObject.Find("Player").GetComponentInChildren<PlayerState>();

        _behaviors = GetComponents<BaseBehavior>();
        _attackBehavior = GetComponent<AttackBehavior>();
        _chaseBehavior = GetComponent<ChaseBehavior>();
        _idleBehavior = GetComponent<IdleBehavior>();
        _patrolBehavior = GetComponent<PatrolBehavior>();
    }

    /* *** MonoBehaviour Methods *** */

    /// <summary>
    /// Simulated player input for aiming and firing.
    /// </summary>
    public override void Update() {
        base.Update();

        if (_attackBehavior != null && _myState.canSeePlayer) {
            if (!_attackBehavior.isActive) {
                _attackBehavior.Activate();
            }
        } else if (_chaseBehavior != null && _myState.didSeePlayer) {
            if (!_chaseBehavior.isActive) {
                _chaseBehavior.Activate();
            }
        } else if (_patrolBehavior != null) {
            if (!_patrolBehavior.isActive) {
                _patrolBehavior.Activate();
            }
        } else if (_idleBehavior != null) {
            if (!_idleBehavior.isActive) {
                _idleBehavior.Activate();
            }
        } else {
            throw new System.MissingMemberException("You must include an IdleBehavior component for " + this.gameObject.name);
        }
    }

    /// <summary>
    /// Move the NPC.
    /// Perform additional functionality that should happen at fixed
    /// intervals in FixedUpdate().
    /// </summary>
    public override void FixedUpdate() {
        _FindPlayer();
        _EstimatePlayerVelocity();

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

        if (_myState.didSeePlayer) {
            _myState.timeSinceDidSeePlayer += Time.fixedDeltaTime;
        } else {
            _myState.timeSinceDidSeePlayer = 0;
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
                _estimatedPlayerVelocity = deltaPosition / Time.fixedDeltaTime;
            }

            _previousPlayerPosition = currentPlayerPosition;
        }
    }
}
