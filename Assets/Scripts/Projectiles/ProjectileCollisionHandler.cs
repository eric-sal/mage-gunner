using UnityEngine;
using System.Collections;

public class ProjectileCollisionHandler : BaseCollisionHandler {
 
    public override void HandleCollision(Collider collidedWith, Vector3 impactVelocity, float distance, Vector3 normal, float deltaTime) {

        // only move the distance until we hit the other collider
        float speed = impactVelocity.magnitude;
        float timeSpentMoving = distance / speed;
        Vector3 tempDistance = impactVelocity * timeSpentMoving;
        Vector3 p = this.transform.position;
        this.transform.position = new Vector3(p.x + tempDistance.x, p.y + tempDistance.y);

        Destroy(this.gameObject, 0.02f);
        this.enabled = false;
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
