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
        _controller.myState.timeSinceDidSeePlayer = 0;
    }

    protected override void _Update() {
        Vector2 position = (Vector2)this.transform.position;
        Vector2 lookAt = position + _controller.character.velocity; // Look in the direction we're moving.

        if (_isChasing) {
            // Since the raycast starts inside our enemy, we want to ignore ourself when casting the ray to find the player.
            LayerMask myLayer = gameObject.layer;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            Vector2 expectedPlayerPosition = _controller.expectedPlayerPosition;
            Vector2 direction = expectedPlayerPosition - position;
            RaycastHit2D hitInfo = Physics2D.Raycast(position, direction, direction.magnitude);
            // Debug.DrawRay(position, direction);

            // If we can we see the expectedPlayerPosition, then look at that point, otherwise
            // look in the direction we're moving. Similar human behavior: looking around corner
            // after existing the room you were in.
            if (hitInfo.collider == null || hitInfo.point == expectedPlayerPosition) {
                lookAt = expectedPlayerPosition;
            }

            gameObject.layer = myLayer;
        }

        _controller.reticle.LerpTo(lookAt, _controller.myState.lookSpeed);
    }

    protected override void _FixedUpdate() {
        _controller.pathfinderAI.MoveAlongPath(_controller.myState.maxWalkSpeed, _HandleOnEndOfPath);
    }
    
    protected void _HandleOnEndOfPath() {
        if (_isChasing) {
            _isChasing = false;

            if (_controller.patrolBehavior != null) {
                // If we have a PatrolBehavior, resume our patrol after we've lost the player.
                _controller.patrolBehavior.Activate();
            } else {
                // If we don't have a PatrolBehavior, return to our starting position after we've lost the player.
                _ReturnToStartingPosition();
            }
        } else {
            // We've reached our starting position, so deactivate ourselves, and look in the
            // direction our startingPosition Waypoint tells us to look.
            _controller.reticle.SetPosition(_controller.myState.startingPosition.lookPosition);
            Deactivate();
        }
    }

    protected void _ReturnToStartingPosition() {
        // We've reached the end of our path, return to our starting position.
        PathfinderAI pathfinder = _controller.pathfinderAI;
        pathfinder.targetPosition = _controller.myState.startingPosition.transform.position;
        pathfinder.RestartPath();
    }
}
