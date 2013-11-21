using UnityEngine;
using System.Collections;

/// <summary>
/// Projectile state.
/// </summary>
public class ProjectileState : MonoBehaviour {

    /* *** Member Variables *** */

    public int damage;          // The amount of damage this projectile does if it collides with an object that takes damage. Set when it's spawned.
    public GameObject spawner;  // GameObject that spawned the projectile

    private MoveableObject _moveableObject;

    /* *** Properties *** */

    public Vector3 velocity {
        get { return _moveableObject.velocity;  }

        set { _moveableObject.velocity = value; }
    }

    /* *** Constructors *** */

    void Awake() {
        _moveableObject = GetComponent<MoveableObject>();
    }
}
