using UnityEngine;
using System.Collections;

/// <summary>
/// Character collision handler.
/// </summary>
public class CharacterCollisionHandler : MonoBehaviour {

    /* *** Member Variables *** */

    protected BaseCharacterState _character;

    /* *** Constructors *** */

    public void Awake() {
        _character = GetComponent<BaseCharacterState>();
    }

    /* *** MonoBehaviour Methods *** */

    public virtual void OnCollisionEnter2D(Collision2D collision) {
        // Debug.Log(string.Format("{0} hit {1}", gameObject.name, collision.gameObject.name));
    }

    public virtual void OnTriggerEnter2D(Collider2D other) {
        // Debug.Log(string.Format("{0} triggered {1}", gameObject.name, other.gameObject.name));

        switch (other.gameObject.tag) {
        case "Projectiles":
            var projectile = other.gameObject.GetComponent<ProjectileState>();
            HandleCollision(projectile);
            break;
        default:
            throw new System.NotImplementedException(string.Format("No handler for tag {0} on gameObject {1}!", other.gameObject.tag, other.gameObject.name));
        }
    }

    /* *** Member Methods *** */

    /// <summary>
    /// Handle collisions with projectiles.
    /// </summary>
    public void HandleCollision(ProjectileState projectile) {
        // TODO: Handle other types of projectiles - grenades, missiles, magic spells, etc.
        if (Object.ReferenceEquals(projectile.spawner, this.gameObject)) {
            return;
        }

        if (projectile.damage > 0) {
            _character.TakeDamage(projectile.damage);
        }
    }
}
