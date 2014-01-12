using UnityEngine;
using System.Collections;

public class ChaseBehavior : BaseBehavior {

    protected bool _isChasing;

    protected override void _Activate() {
        _isChasing = true;
        _controller.pathfinderAI.targetPosition = _controller.myState.playerPosition;
        _controller.pathfinderAI.RestartPath();
    }

    protected override void _Deactivate() {
        _controller.myState.didSeePlayer = false;
    }

    protected override void _FixedUpdate() {
        _controller.pathfinderAI.MoveAlongPath(_controller.myState.maxWalkSpeed, _HandleOnEndOfPath);
    }
    
    protected void _HandleOnEndOfPath() {
        if (_isChasing) {
            _isChasing = false;

            // If we don't have a PatrolBehavior, return to our starting position after
            // we've lost the player.
            if (_controller.patrolBehavior != null) {
                // If we have a PatrolBehavior, resume our patrol after we've lost the player.
                _controller.patrolBehavior.Activate();
            } else {
                _ReturnToStartingPosition();
            }
        } else {
            // we've reached our starting position, so deactivate ourselves
            Deactivate();
        }
    }

    protected void _ReturnToStartingPosition() {
        // We've reached the end of our path, return to our starting position
        PathfinderAI pathfinder = _controller.pathfinderAI;
        pathfinder.targetPosition = _controller.myState.startingPosition;
        pathfinder.RestartPath();
    }
}
