using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

public class CoverSeekingBehavior : AttackBehavior {
    protected override void _Activate() {
        var targets = new List<Vector3>(SceneController.activeCoverWaypoints.Count);
        
        foreach (CoverWaypoint wp in SceneController.activeCoverWaypoints) {
            if (wp.isViable) {
                targets.Add(wp.transform.position);
            }
        }

        if (targets.Count > 0) {
            StartCoroutine(_controller.pathfinderAI.TakeShortestPath(targets));
        } else {
            Deactivate();
        }
    }

    protected override void _Deactivate() {
        _controller.character.velocity = Vector3.zero;
        _rigidbody.velocity = Vector3.zero;
    }
    
    protected override void _Update() {
        _controller.reticle.LerpTo(_controller.playerState.aimPoint, _controller.myState.lookSpeed);
        //_controller.myState.LookAt(_controller.playerState.aimPoint);
    }
    
    protected override void _FixedUpdate() {
        _controller.pathfinderAI.MoveAlongPath(_controller.myState.maxWalkSpeed, _HandleOnEndOfPath);
    }
    
    protected void _HandleOnEndOfPath() {
        Deactivate();
    }
}
