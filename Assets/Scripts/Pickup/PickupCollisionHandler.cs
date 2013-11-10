using UnityEngine;
using System.Collections;

public class PickupCollisionHandler : BaseCollisionHandler {

    public override void HandleCollision(Collider collidedWith, Vector3 impactVelocity, float distance, Vector3 normal, float deltaTime) {

        AudioSource pickupSound = this.GetComponent<AudioSource>();
        if (pickupSound != null) {
            pickupSound.Play();

            // disable collisions while the sound is playing
            this.collider.enabled = false;
            this.renderer.enabled = false;
            this.enabled = false;

            // destroy after the sound is finished
            Destroy(this.gameObject, pickupSound.clip.length);
        }
        else {
            Destroy(this.gameObject);
        }
    }
}
