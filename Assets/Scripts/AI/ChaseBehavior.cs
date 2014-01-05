using UnityEngine;
using System.Collections;

public class ChaseBehavior : BaseBehavior {

    const float MIN_TARGET_DISTANCE = 1f;

    public ChaseBehavior(NpcController controller) : base(controller) {
        _controller.pathfinderAI.targetPosition = _controller.myState.playerPosition;
        _controller.pathfinderAI.RestartPath();
    }

    public override void doUpdate() {
        // Do nothing
    }
    
    public override void doFixedUpdate() {
        _controller.pathfinderAI.MoveAlongPath(_controller.myState.maxWalkSpeed);
    }

    public override INpcBehavior GetNextBehavior() {

        NpcState myState = _controller.myState;
        INpcBehavior nextBehavior = base.GetNextBehavior();

        if (nextBehavior is IdleBehavior) {
            Vector2 diff = myState.startingPosition - (Vector2)_controller.transform.position;
            if (diff.sqrMagnitude >= MIN_TARGET_DISTANCE) {
                // we haven't reached our starting position, stay in ChaseBehavior
                nextBehavior = this;
            }
        }

        if (!(nextBehavior is ChaseBehavior)) {
            return nextBehavior;
        }

        if (myState.didSeePlayer) {
            PathfinderAI pathfinder = _controller.pathfinderAI;
            Vector2 diff = pathfinder.targetPosition - (Vector2)_controller.transform.position;
            if (diff.sqrMagnitude < MIN_TARGET_DISTANCE) {
                // We've reached the end of our path, return to our starting position
                myState.didSeePlayer = false;
                pathfinder.targetPosition = myState.startingPosition;
                pathfinder.RestartPath();
            }
        }

        return this;
    }
}
