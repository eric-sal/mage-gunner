using UnityEngine;
using System.Collections.Generic;

public class CoverSeekingBehavior : AttackBehavior {

    public CoverWaypoint targetWaypoint;

    protected override void _Activate() {

        if (_controller.pathfinderAI == null) {
            throw new MissingComponentException("missing PathfinderAI component on " + gameObject.name);
        }
        _FindCover();
    }

    protected override void _Deactivate() {
        _controller.pathfinderAI.ResetPath();
    }

    protected override void _Update() {
        
        _controller.reticle.LerpTo(_controller.playerState.aimPoint, _controller.myState.lookSpeed);

        if (this.targetWaypoint != null && !this.targetWaypoint.isViable) {
            _FindCover();
        }

    }
    
    protected override void _FixedUpdate() {
        if (_controller.pathfinderAI.hasReachedEndOfPath) {
            _HandleOnEndOfPath();
        }
    }

    protected void _FindCover() {
        int listCapacity = SceneController.activeCoverWaypoints.Count;

        var viableWaypoints = new List<CoverWaypoint>(listCapacity);
        var targets = new List<Vector3>(listCapacity);

        foreach (CoverWaypoint wp in SceneController.activeCoverWaypoints) {
            if (wp.isViable) {
                viableWaypoints.Add(wp);
                targets.Add(wp.transform.position);
            }
        }

        if (targets.Count > 0) {
            Vector3 target = _controller.pathfinderAI.TakeShortestPath(targets);
            foreach (CoverWaypoint wp in viableWaypoints) {
                if (wp.transform.position == target) {
                    this.targetWaypoint = wp;
                    break;
                }
            }
        } else {
            Deactivate();
        }
    }
    
    protected void _HandleOnEndOfPath() {
        Deactivate();
    }
}
