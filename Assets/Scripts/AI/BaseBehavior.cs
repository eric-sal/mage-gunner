using UnityEngine;
using System.Collections;

public class BaseBehavior : INpcBehavior {

    protected NpcController _controller;
    
    public BaseBehavior(NpcController controller) {
        _controller = controller;
    }

    public virtual void doUpdate() {
        // Do nothing
    }
    
    public virtual void doFixedUpdate() {
        // Do nothing
    }

    public virtual INpcBehavior GetNextBehavior() {

        INpcBehavior behavior = this;
        
        if (_controller.myState.canSeePlayer) {
            if (!(this is AttackBehavior)) {
                behavior = new AttackBehavior(_controller);
            }
        } else if (_controller.myState.didSeePlayer) {
            if (!(this is ChaseBehavior)) {
                behavior = new ChaseBehavior(_controller);
            }
        } else if (_controller.pathfinderAI.firstWaypoint != null) {
            if (!(this is PatrolBehavior)) {
                behavior = new PatrolBehavior(_controller);
            }
        } else if (!(this is IdleBehavior)) {
            behavior = new IdleBehavior(_controller);
        }
        
        return behavior;
    }

}
