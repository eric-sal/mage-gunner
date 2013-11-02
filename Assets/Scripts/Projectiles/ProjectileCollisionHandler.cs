using UnityEngine;
using System.Collections;

public class ProjectileCollisionHandler : BaseCollisionHandler {
 
    public override void HandleCollision(Collider collidedWith, Vector3 impactVelocity, float distance, Vector3 normal, float deltaTime) {
        //Debug.Log(string.Format("{0} collided with {1}", this.name, collidedWith.name));
        Destroy(this.gameObject);
    }

    public override void HandleCollision(CharacterCollisionHandler other, Vector3 impactVelocity, float distance, Vector3 normal, float deltaTime) {
        if (other.gameObject.name == "Player") {
            // ignore collisions with the player for now
            return;
        }
     
        DefaultHandleCollision(other, impactVelocity, distance, normal, deltaTime);
    }

    public override void HandleCollision(ProjectileCollisionHandler other, Vector3 impactVelocity, float distance, Vector3 normal, float deltaTime) {
        // ignore collisions with other projectiles
        return;
    }
}
