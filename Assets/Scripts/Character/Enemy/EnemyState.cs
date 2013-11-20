using UnityEngine;
using System.Collections;

/// <summary>
/// Enemy state.
/// </summary>
public class EnemyState : BaseCharacterState {

    /* *** Member Variables *** */

    public bool anticipatePlayerMovement = true;    // Mostly for debugging. Can probably remove this later.
	public int lookSpeed = 2;   // How quickly the NPC looks at the player.

    /* *** Protected Variables *** */

    /// <summary>
    /// Handle player death.
    /// TODO: We probably don't want to just destroy the instance.
    /// </summary>
    protected override void Die() {
        Destroy(this.gameObject);
    }
}
