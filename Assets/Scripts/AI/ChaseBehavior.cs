using UnityEngine;
using System.Collections;

public class ChaseBehavior : BaseBehavior {

    const float MIN_TARGET_DISTANCE = 1f;

    protected bool _isChasing;

    protected override void _Activate() {
        _isChasing = true;
        _controller.pathfinderAI.targetPosition = _controller.myState.playerPosition;
        _controller.pathfinderAI.RestartPath();
    }

    protected override void _Deactivate() {
        _controller.myState.didSeePlayer = false;
    }

    protected override void _Update() {
        NpcState myState = _controller.myState;
        Vector2 diff;

        if (_isChasing) {
            PathfinderAI pathfinder = _controller.pathfinderAI;
            diff = pathfinder.targetPosition - (Vector2)_controller.transform.position;
            if (diff.sqrMagnitude < MIN_TARGET_DISTANCE) {
                // We've reached the end of our path, return to our starting position
                _isChasing = false;
                pathfinder.targetPosition = myState.startingPosition;
                pathfinder.RestartPath();
            }
        } else if (_controller.patrolBehavior != null) {
            // If we have a PatrolBehavior, resume our patrol after we've lost the player.
            _controller.patrolBehavior.Activate();
        } else {
            // If we don't have a PatrolBehavior, return to our starting position after
            // we've lost the player.
            diff = myState.startingPosition - (Vector2)_controller.transform.position;
            if (diff.sqrMagnitude < MIN_TARGET_DISTANCE) {
                // we've reached our starting position, so deactivate ourselves
                Deactivate();
            }
        }
    }

    protected override void _FixedUpdate() {
        _controller.pathfinderAI.MoveAlongPath(_controller.myState.maxWalkSpeed);
    }
}
