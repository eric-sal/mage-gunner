using UnityEngine;
using System.Collections;

public class ProjectileCollisionHandler : BaseCollisionHandler {
    private ProjectileState _projectile;

    public override void Awake() {
        base.Awake();
        _projectile = GetComponent<ProjectileState>();
    }
 
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
        if (Object.ReferenceEquals(other.gameObject, _projectile.spawner)) {
            // ignore collisions with the GameObject that spawned this projectile
            return;
        }
     
        DefaultHandleCollision(other, impactVelocity, distance, normal, deltaTime);
    }

    public override void HandleCollision(ProjectileCollisionHandler other, Vector3 impactVelocity, float distance, Vector3 normal, float deltaTime) {
        // ignore collisions with other projectiles
        Vector3 p = this.transform.position;
        Vector3 d = _projectile.velocity * deltaTime;
        this.transform.position = new Vector3(p.x + d.x, p.y + d.y);
    }
}
