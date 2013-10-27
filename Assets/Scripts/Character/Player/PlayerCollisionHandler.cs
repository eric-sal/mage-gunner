using UnityEngine;
using System.Collections;

public class PlayerCollisionHandler : CharacterCollisionHandler {

    private PlayerCharacterController _controller;
 
    public override void Awake() {
        base.Awake();
        //_controller = GetComponent<PlayerCharacterController>();
    }
	/* Left here as an example
    public override void HandleCollision(MarioTwinCollisionHandler other, Vector3 fromDirection, float distance, Vector3 normal) {
        HandleCollision(other.collider, fromDirection, distance, Vector3 normal);
        if (fromDirection == Vector3.down) {
            // If the player jumps on the head of the twin, make him bounce off
            _controller.Jump();
        }
    }

    public override void HandleCollision(PickupCollisionHandler other, Vector3 fromDirection, float distance, Vector3 normal) {
        // treat all pickups as coins for now
        _character.coinCount++;
    }
    */
}
