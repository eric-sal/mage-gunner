using UnityEngine;
using System.Collections;

/// <summary>
/// Projectile collision handler.
/// </summary>
public class ProjectileCollisionHandler : BaseCollisionHandler {

    /* *** Member Variables *** */

    private ProjectileState _projectile;

    /* *** Constructors *** */

    public override void Awake() {
        base.Awake();
        _projectile = GetComponent<ProjectileState>();
    }

    /* *** Public Methods *** */

    /// <summary>
    /// Default collision handler.
    /// </summary>
    /// <param name='collidedWith'>
    /// The collider we hit.
    /// </param>
    /// <param name='impactVelocity'>
    /// Impact velocity.
    /// </param>
    /// <param name='distance'>
    /// Distance from the other collider's origin to the impact point.
    /// </param>
    /// <param name='normal'>
    /// Normal of the surface the ray hit.
    /// </param>
    /// <param name='deltaTime'>
    /// Delta time.
    /// </param>
    public override void HandleCollision(Collider collidedWith, Vector3 impactVelocity, float distance, Vector3 normal, float deltaTime) {

        // only move the distance until we hit the other collider
        float speed = impactVelocity.magnitude;
        float timeSpentMoving = distance / speed;
        Vector3 d = impactVelocity * timeSpentMoving;
        Vector3 p = this.transform.position;
        this.transform.position = new Vector3(p.x + d.x, p.y + d.y);

        Destroy(this.gameObject, 0.02f);
        this.enabled = false;
    }

    /// <summary>
    /// Handles collisions with a Character.
    /// </summary>
    /// <param name='collidedWith'>
    /// The collider we hit.
    /// </param>
    /// <param name='impactVelocity'>
    /// Impact velocity.
    /// </param>
    /// <param name='distance'>
    /// Distance from the other collider's origin to the impact point.
    /// </param>
    /// <param name='normal'>
    /// Normal of the surface the ray hit.
    /// </param>
    /// <param name='deltaTime'>
    /// Delta time.
    /// </param>
    public override void HandleCollision(CharacterCollisionHandler other, Vector3 impactVelocity, float distance, Vector3 normal, float deltaTime) {
        if (Object.ReferenceEquals(other.gameObject, _projectile.spawner)) {
            // ignore collisions with the GameObject that spawned this projectile
            return;
        }
     
        DefaultHandleCollision(other, impactVelocity, distance, normal, deltaTime);
    }

    /// <summary>
    /// Handles collisions with other projectiles.
    /// </summary>
    /// <param name='collidedWith'>
    /// The collider we hit.
    /// </param>
    /// <param name='impactVelocity'>
    /// Impact velocity.
    /// </param>
    /// <param name='distance'>
    /// Distance from the other collider's origin to the impact point.
    /// </param>
    /// <param name='normal'>
    /// Normal of the surface the ray hit.
    /// </param>
    /// <param name='deltaTime'>
    /// Delta time.
    /// </param>
    public override void HandleCollision(ProjectileCollisionHandler other, Vector3 impactVelocity, float distance, Vector3 normal, float deltaTime) {
        // ignore collisions with other projectiles
        Vector3 p = this.transform.position;
        Vector3 d = _projectile.velocity * deltaTime;
        this.transform.position = new Vector3(p.x + d.x, p.y + d.y);
    }
}
