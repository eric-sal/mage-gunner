using UnityEngine;
using System.Collections;

/// <summary>
/// Enemy state.
/// </summary>
public class EnemyState : BaseCharacterState {

    /* *** Member Variables *** */

    public bool anticipatePlayerMovement = true;    // Mostly for debugging. Can probably remove this later.
    public int fieldOfVision;   // In degrees
	public int lookSpeed = 2;   // How quickly the NPC looks at the player.
    public float sightDistance;  // How far can the enemy see? TODO: distance is affected by enemy state. ie: see "further" when actively searching vs passive state.

    /* *** Member Methods *** */

    /// <summary>
    /// Handle player death.
    /// TODO: We probably don't want to just destroy the instance.
    /// </summary>
    protected override void _Die() {
        Destroy(this.gameObject);
    }
}
