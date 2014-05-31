using UnityEngine;
using System.Collections;

/// <summary>
/// Projectile collision handler.
/// </summary>
public class ProjectileCollisionHandler : MonoBehaviour {

    /* *** Member Variables *** */
    protected int _layerMask;
    protected ProjectileState _projectile;

    /* *** Constructors *** */

    public void Awake() {
        _layerMask = ~(1 << LayerMask.NameToLayer("Projectiles"));
        _projectile = GetComponent<ProjectileState>();
    }

    /* *** MonoBehaviour Methods *** */

    public void FixedUpdate() {

        if (this.rigidbody.velocity == Vector3.zero) {
            // The projectile was stopped in a previous call to FixedUpdate
            return;
        }

        LayerMask spawnerLayer = _layerMask;

        if (_projectile.spawner != null) {
            // We want to ignore the gameObject that spawned the projectile, so temporarily put
            // that gameObject on our "Projectiles" layer.
            spawnerLayer = _projectile.spawner.layer;
            _projectile.spawner.layer = LayerMask.NameToLayer("Projectiles");
        }

        Vector3 velocity = this.rigidbody.velocity;
        float distance = Mathf.Abs(velocity.magnitude * Time.fixedDeltaTime);

        RaycastHit hitInfo;
        if (Physics.Raycast(this.transform.position, velocity, out hitInfo, distance, _layerMask)) {
            if (hitInfo.collider != null) {
                // the project hit something, stop it
                _projectile.transform.position = hitInfo.point;
                this.rigidbody.velocity = Vector3.zero;
            }
        }

        if (_projectile.spawner != null) {
            // If we have a non-null spawner, then set its layer back to the original layer.
            _projectile.spawner.layer = spawnerLayer;
        }
    }
}
