using UnityEngine;
using System.Collections;

/// <summary>
/// Projectile collision handler.
/// </summary>
public class ProjectileCollisionHandler : MonoBehaviour {

    /* *** Class Variables *** */
    private static int _layerMask = ~(1 << LayerMask.NameToLayer("Projectiles"));

    /* *** Member Variables *** */

    private ProjectileState _projectile;

    /* *** Constructors *** */

    public void Awake() {
        _projectile = GetComponent<ProjectileState>();
    }

    /* *** MonoBehaviour Methods *** */

    public void FixedUpdate() {
        LayerMask spawnerLayer = _layerMask;

        if (_projectile.spawner != null) {
            // We want to ignore the gameObject that spawned the projectile, so temporarily put
            // that gameObject on our "Projectiles" layer.
            spawnerLayer = _projectile.spawner.layer;
            _projectile.spawner.layer = LayerMask.NameToLayer("Projectiles");
        }

        Vector2 velocity = this.rigidbody2D.velocity;
        float distance = Mathf.Abs(velocity.magnitude * Time.fixedDeltaTime);
        RaycastHit2D hitInfo = Physics2D.Raycast(this.transform.position.Vector2D(), velocity, distance, _layerMask);
        if (hitInfo.collider != null) {
            _projectile.transform.position = hitInfo.point.Vector3D();
            this.rigidbody2D.velocity = Vector2.zero;
        }

        if (_projectile.spawner != null) {
            // If we have a non-null spawner, then set its layer back to the original layer.
            _projectile.spawner.layer = spawnerLayer;
        }
    }
}
