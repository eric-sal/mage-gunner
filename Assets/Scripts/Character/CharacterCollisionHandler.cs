using UnityEngine;
using System.Collections;

/// <summary>
/// Character collision handler.
/// Inherits from BaseCollisionHandler.
/// </summary>
public class CharacterCollisionHandler : BaseCollisionHandler {

    /* *** Member Variables *** */

    protected BaseCharacterState _character;

    /* *** Constructors *** */

    public override void Awake() {
        base.Awake();
        _character = GetComponent<BaseCharacterState>();
    }

    /* *** Member Methods *** */

    /// <summary>
    /// Default collision handler.
    /// Stop the character's movement in the direction the collision came from.
    /// </summary>
    public override void HandleCollision(Collider collidedWith, Vector3 impactVelocity, float distance, Vector3 normal, float deltaTime) {
        throw new System.NotImplementedException("Was not expecting this to be called!");
    }

    /// <summary>
    /// Handle collisions with projectiles.
    /// </summary>
    public override void HandleCollision(ProjectileCollisionHandler other, Vector3 impactVelocity, float distance, Vector3 normal, float deltaTime) {
        // TODO: Handle other types of projectiles - grenades, missiles, magic spells, etc.
        ProjectileState projectile = other.GetComponent<ProjectileState>();

        if (Object.ReferenceEquals(projectile.spawner, this.gameObject)) {
            return;
        }

        if (projectile.damage > 0) {
            _character.TakeDamage(projectile.damage);
        }
    }
}