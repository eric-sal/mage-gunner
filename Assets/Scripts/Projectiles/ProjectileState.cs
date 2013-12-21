using UnityEngine;
using System.Collections;

/// <summary>
/// Projectile state.
/// </summary>
public class ProjectileState : MonoBehaviour {

    /* *** Member Variables *** */

    public int damage;          // The amount of damage this projectile does if it collides with an object that takes damage. Set when it's spawned.
    public GameObject spawner;  // GameObject that spawned the projectile
    public Vector2 velocity;
}
