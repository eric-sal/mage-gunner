using UnityEngine;
using System.Collections.Generic;

public class PatrolBehavior : BaseBehavior {

    const int FORWARD = 1;
    const int BACKWARD = -1;

    /* *** Member Variables *** */

    public Waypoint firstWaypoint;  //The first waypoint to calculate the path to

    private int _currentWaypoint = 0; //The waypoint object we are currently moving toward
    private int _direction = FORWARD; //A 1 indicates we are moving "forwards" on the route and a -1 indicates "backwards"
    private List<Waypoint> _waypoints;


    /* *** Constructors *** */

    void Awake() {
        if (this.firstWaypoint == null) {
            throw new MissingReferenceException("PatrolBehavior requires at least one Waypoint!");
        }

        _waypoints = new List<Waypoint>();
        Waypoint next = this.firstWaypoint;
        while (next != null) {
            _waypoints.Add(next);
            next = next.next;
        }
    }
    
    protected Waypoint _NextWaypoint {
        get {
            int nextWaypoint = _currentWaypoint + _direction;
            if (nextWaypoint >= _waypoints.Count || nextWaypoint < 0) {
                // change direction
                nextWaypoint = _currentWaypoint + _direction * -1;
            }
            return _waypoints[nextWaypoint];
        }
    }

    /* *** Member Methods *** */

    protected override void _Activate() {
        PathfinderAI pathfinder = _controller.pathfinderAI;
        pathfinder.targetPosition = _waypoints[_currentWaypoint].transform.position;
        pathfinder.RestartPath();
    }

    protected override void _Update() {
        BaseCharacterState character = _controller.character;

        // If we can't see the player, aim in the direction we're moving.
        if (character.velocity != Vector3.zero) {
            // Because the velocity vector describes a unit vector from the origin, we have to
            // translate it to the NPC's current position.
            // NOTE: The 2 is an arbitrary scalar. Should that value be _myState.sightDistance?
            // Or can we keep it as a unit vector? How does the distance of the reticle from the
            // NPC affect the speed with which they aim at the player? My hunch is that by using
            // Vector2.Lerp, it makes no difference. If we were to move the the reticle by setting
            // a velocity though, the distance would make a difference.
            Vector3 velocity = character.velocity;
            Vector3 position = character.transform.position;
            _controller.reticle.LerpTo(position + velocity * 2, _controller.myState.lookSpeed);
        }
    }

    protected override void _FixedUpdate() {
        _controller.pathfinderAI.MoveAlongPath(_controller.myState.maxWalkSpeed, _UpdateTargetPosition);
    }

    /// <summary>
    /// Sets the target position equal to the next waypoint
    /// </summary>
    protected void _UpdateTargetPosition() {

        if (_waypoints.Count == 0) {
            return;
        }

        PathfinderAI pathfinder = _controller.pathfinderAI;

        int nextWaypoint = _waypoints.IndexOf(_NextWaypoint);
        _direction = nextWaypoint - _currentWaypoint;
        _currentWaypoint = nextWaypoint;
        pathfinder.targetPosition = _waypoints[_currentWaypoint].transform.position;
        pathfinder.RestartPath();
    }
}
