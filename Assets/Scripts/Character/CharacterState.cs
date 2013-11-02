using UnityEngine;
using System.Collections;

/// <summary>
/// The Character state is modified by a class that inherits from AbstractCharacterController.
/// </summary>
public class CharacterState : MonoBehaviour {

    public Vector2 position;
    public Vector2 facing;
    public Vector2 velocity;
    public float maxWalkSpeed;
    public int health;
    public bool isWalking;

    public int strength;


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
    public float getRecoilReduction() {
        return strength * 0.05f;
    }

}
