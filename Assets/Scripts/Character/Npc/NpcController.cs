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
    protected Vector3 _estimatedPlayerVelocity;
    protected IdleBehavior _idleBehavior;
    protected PathfinderAI _pathfinderAI;
    protected int _lineOfSightLayerMask;
    protected int _lineOfFireLayerMask;
    protected NpcState _myState;
    protected PatrolBehavior _patrolBehavior;
    protected PlayerState _playerState;
    protected Vector3 _previousPlayerPosition = Vector3.zero;

    public BaseBehavior[] behaviors {
        get { return _behaviors; }
    }

    public Vector3 estimatedPlayerVelocity {
        get { return _estimatedPlayerVelocity; }
    }

    // The expectedPlayerPosition is where the NPC expects the player to be based on
    // the last known player position, the estimated velocity, and the amount of time
    // that has passed since they last saw the player.
    public Vector3 expectedPlayerPosition {
        get {
            Vector3 expected;
            Vector3 estimatedDistanceTraveled = estimatedPlayerVelocity * _myState.timeSinceDidSeePlayer;

            // Since the NPC doesn't have any spatial reasoning, we'll fake it for them
            // to hopefully make the NPCs *look* smart.
            // Cast a ray from the last known player position to our expected player
            // position. If we hit an obstacle, then we know that the player couldn't
            // have moved through it. This may provide more info to the NPC than they
            // would/should normally have, but the intention is to create the illusion
            // of intelligence, so we'll live with it.
            RaycastHit hitInfo;
            expected = _myState.lastKnownPlayerPosition + estimatedDistanceTraveled;
            if (Physics.Raycast(_myState.lastKnownPlayerPosition, estimatedDistanceTraveled, out hitInfo, estimatedDistanceTraveled.magnitude)) {
                if (hitInfo.collider != null) {
                    expected = hitInfo.point;
                }
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
        _lineOfSightLayerMask = ((1 << LayerMask.NameToLayer("Players")) | (1 << LayerMask.NameToLayer("Obstacles")) | (1 << LayerMask.NameToLayer("Enemies")));
//        _lineOfFireLayerMask = _lineOfSightLayerMask | (1 << LayerMask.NameToLayer("HalfCover"));

        _myState = (NpcState)_character;
        _pathfinderAI = GetComponent<PathfinderAI>();
        _playerState = GameObject.Find("Player").GetComponentInChildren<PlayerState>();

        _behaviors = GetComponents<BaseBehavior>();
        _attackBehavior = GetComponent<AttackBehavior>();
        _chaseBehavior = GetComponent<ChaseBehavior>();
        _idleBehavior = GetComponent<IdleBehavior>();
        _patrolBehavior = GetComponent<PatrolBehavior>();
    }

    public void Start() {
//        if (_pathfinderAI != null) {
//            _pathfinderAI.targetPosition = _myState.startingPosition.transform.position;
//        }
    }

    /* *** MonoBehaviour Methods *** */

    /// <summary>
    /// Simulated player input for aiming and firing.
    /// </summary>
    public override void Update() {
        base.Update();

        if (_attackBehavior != null && (_myState.canSeePlayer || _attackBehavior.isActive)) {
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

    public void OnTriggerEnter(Collider other) {
        if (other.name == "VisibleArea") {
            _myState.onScreen = true;
        }
    }
    
    public void OnTriggerExit(Collider other) {
        if (other.name == "VisibleArea") {
            _myState.onScreen = false;
        }
    }

    /// <summary>
    /// Try to find the player on the screen.
    /// </summary>
    protected void _FindPlayer() {

        bool canSeePlayer = false;

        if (_myState.onScreen) {

            // Since the raycast starts inside our enemy, we want to ignore ourself when casting the ray to find the player.
            LayerMask myLayer = gameObject.layer;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            
            Vector3 playerAimPoint = _playerState.aimPoint;
            Vector3 npcAimPoint = _myState.aimPoint;
            
            Vector3 npcToPlayerVector = playerAimPoint - npcAimPoint;
            
            Debug.DrawRay(npcAimPoint, npcToPlayerVector, Color.yellow);

            // is there anything obstructing our view of the player?
            RaycastHit hitInfo;
            if (Physics.Raycast(npcAimPoint, npcToPlayerVector, out hitInfo, npcToPlayerVector.sqrMagnitude, _lineOfSightLayerMask)) {
                if (Object.ReferenceEquals(hitInfo.collider.gameObject, _playerState.gameObject)) {
                    canSeePlayer = true;
                    _myState.lastKnownPlayerPosition = _playerState.transform.position;
                }
            }

            gameObject.layer = myLayer;
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
                _estimatedPlayerVelocity = deltaPosition / Time.fixedDeltaTime;
            }

            _previousPlayerPosition = currentPlayerPosition;
        }
    }

    public override bool WillPopUp(Vector3 fromDirection) {
        return false;
    }
}
