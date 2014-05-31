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

    public Firearm equippedFirearm;
    public int health;
    public bool kneeling = false;
    public Vector3 lookDirection;  // Direction the character is looking.
    public float maxWalkSpeed;
    public int strength; // 10 is average strength
    public Vector3 velocity;
    public bool isDodging;

    /* *** Properties *** */

    public Vector3 aimPoint {
        get {
            float height = this.transform.localScale.z;
            return this.transform.position - new Vector3(0, 0, height * 0.3f);
        }
    }

    public float recoilReductionRate {
        // TODO: Figure out how to do this correctly
        get { return (5 * this.strength + 8) * Time.fixedDeltaTime; }
    }

    /* *** Member Methods *** */

    /// <summary>
    /// Looks at position.
    /// </summary>
    /// <param name='position'>
    /// Position to look at.
    /// </param>
    public void LookAt(Vector3 position) {
        this.lookDirection = position - this.aimPoint;
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
            _Die();
        }
    }

    /// <summary>
    /// Define in subclasses.
    /// Handle what happens to the character when they die.
    /// </summary>
    protected abstract void _Die();
}
