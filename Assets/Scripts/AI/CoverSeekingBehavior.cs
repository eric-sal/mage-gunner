using UnityEngine;
using System.Collections;

public class CoverSeekingBehavior : AttackBehavior {

    protected override void _Activate() {
        _controller.pathfinderAI.targetPosition = GameObject.Find("CoverWaypoint").transform.position;
        _controller.pathfinderAI.RestartPath();
    }
    
    protected override void _Deactivate() {

    }
    
    protected override void _Update() {
        _controller.reticle.LerpTo(_controller.playerState.aimPoint, _controller.myState.lookSpeed);
        //_controller.myState.LookAt(_controller.playerState.aimPoint);
    }
    
    protected override void _FixedUpdate() {
        _controller.pathfinderAI.MoveAlongPath(_controller.myState.maxWalkSpeed, _HandleOnEndOfPath);
    }
    
    protected void _HandleOnEndOfPath() {

    }

}
