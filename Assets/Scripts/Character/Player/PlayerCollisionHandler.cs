using UnityEngine;
using System.Collections;

public class PlayerCollisionHandler : CharacterCollisionHandler {

    private PlayerCharacterController _controller;
 
    public override void Awake() {
        base.Awake();
        _controller = GetComponent<PlayerCharacterController>();
    }
	
    public override void HandleCollision(MarioTwinCollisionHandler other, Vector3 fromDirection, float distance) {
        HandleCollision(other.collider, fromDirection, distance);
        if (fromDirection == Vector3.down) {
            // If the player jumps on the head of the twin, make him bounce off
            _controller.Jump();
        }
    }

    public override void HandleCollision(PickupCollisionHandler other, Vector3 fromDirection, float distance) {
        // treat all pickups as coins for now
        _character.coinCount++;
    }
}
