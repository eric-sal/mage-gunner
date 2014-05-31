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
    public int fieldOfVision = 90;   // In degrees
	public int lookSpeed = 2;   // How quickly the NPC looks at the player.
    public Vector3 lastKnownPlayerPosition; // The last place we saw the player.  Updated continuously while we can see them.
    public float sightDistance = 20; // How far can the NPC see? TODO: distance is affected by NPC state. ie: see "further" when actively searching vs passive state.
    public Waypoint startingPosition; // The NPC's starting position
    public float timeSinceDidSeePlayer; // How long has it been since we've seen the player?

    /* *** Constructors *** */

    public void Awake() {
        if (this.startingPosition == null) {
            // startingPosition should be a Waypoint. If it is null, we'll create a new Waypoint for this NPC on the fly.
            this.startingPosition = Waypoint.Create(this.transform.position);
            this.startingPosition.lookDirection = this.lookDirection;
        }
    }

    /* *** Member Methods *** */

    /// <summary>
    /// Handle player death.
    /// TODO: We probably don't want to just destroy the instance.
    /// </summary>
    protected override void _Die() {
        Destroy(this.gameObject);
    }
}
