using UnityEngine;
using System.Collections;

/// <summary>
/// The Character state is modified by a class that inherits from BaseCharacterController.
/// </summary>
public class CharacterState : MonoBehaviour {

    public Vector3 facing;
    public int health;
    public bool isWalking;
    public float maxWalkSpeed;
    public int strength;
    public Vector3 velocity;

    public bool isMovingRight {
        get { return velocity.x > 0; }
    }

    public bool isMovingLeft {
        get { return velocity.x < 0; }
    }

    public bool isMovingUp {
        get { return velocity.y > 0; }
    }

    public bool isMovingDown {
        get { return velocity.y < 0; }
    }

    // TODO: pass in some kind of parameter for weapon?
    public float GetRecoilReduction() {
        return strength * 0.05f;
    }

    public void TakeDamage(int damage) {
        health -= damage;

        if (health < 0) {
            Die();
        }
    }

    public void Die() {
        Destroy(this.gameObject);
    }
}
