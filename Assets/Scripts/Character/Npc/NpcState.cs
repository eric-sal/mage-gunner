using UnityEngine;
using System.Collections;

/// <summary>
/// NPC state.
/// </summary>
public class NpcState : BaseCharacterState {

    /* *** Member Variables *** */

    public bool anticipatePlayerMovement = true;    // Mostly for debugging. Can probably remove this later.
    public int fieldOfVision;   // In degrees
	public int lookSpeed = 2;   // How quickly the NPC looks at the player.
    public float sightDistance;  // How far can the NPC see? TODO: distance is affected by NPC state. ie: see "further" when actively searching vs passive state.

    /* *** Member Methods *** */

    /// <summary>
    /// Handle player death.
    /// TODO: We probably don't want to just destroy the instance.
    /// </summary>
    protected override void _Die() {
        Destroy(this.gameObject);
    }
}
