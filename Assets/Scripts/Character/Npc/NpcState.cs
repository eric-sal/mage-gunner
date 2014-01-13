using UnityEngine;
using System.Collections;

/// <summary>
/// NPC state.
/// </summary>
public class NpcState : BaseCharacterState {

    /* *** Member Variables *** */

    public bool anticipatePlayerMovement = true;    // Mostly for debugging. Can probably remove this later.
    public bool canSeePlayer;   // Can we see the player?
    public bool didSeePlayer;   // True when canSeePlayer changes from true to false, changes to False when we can see the player
    public int fieldOfVision;   // In degrees
	public int lookSpeed = 2;   // How quickly the NPC looks at the player.
    public Vector2 playerPosition; // Last known player position
    public float sightDistance; // How far can the NPC see? TODO: distance is affected by NPC state. ie: see "further" when actively searching vs passive state.
    public Waypoint startingPosition; // The NPC's starting position
    public float timeSinceDidSeePlayer; // How long has it been since we've seen the player?

    /* *** Member Methods *** */

    /// <summary>
    /// Handle player death.
    /// TODO: We probably don't want to just destroy the instance.
    /// </summary>
    protected override void _Die() {
        Destroy(this.gameObject);
    }
}
