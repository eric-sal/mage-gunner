using UnityEngine;
using System.Collections;

/// <summary>
/// The Character state is modified by a class that inherits from
/// BaseCharacterController.
///
/// You cannot assign this to a game object directly.  You must
/// inherit from this class.
/// </summary>
public abstract class BaseCharacterState : MonoBehaviour {

    /* *** Member Variables *** */

    public int health;
    public float maxWalkSpeed;
    public int strength;

    /* *** Public Methods *** */

    /// <summary>
    /// Gets the recoil reduction amount.
    /// Directly related to the strength of the player.
    /// </summary>
    /// <returns>
    /// The recoil reduction.
    /// </returns>
    public float GetRecoilReduction() {
        return this.strength * 0.05f;
    }

    /// <summary>
    /// Reduce the player's health by the specified amout of damage.
    /// </summary>
    /// <param name='damage'>
    /// The amount of damage to apply.
    /// </param>
    public void TakeDamage(int damage) {
        this.health -= damage;

        if (this.health < 0) {
            // If the player doesn't have any more health, they're dead.
            Die();
        }
    }

    /* *** Protected Methods *** */

    /// <summary>
    /// Define in subclasses.
    /// Handle what happens to the character when they die.
    /// </summary>
    protected abstract void Die();
}
