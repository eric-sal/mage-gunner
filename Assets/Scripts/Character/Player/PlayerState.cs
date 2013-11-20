using UnityEngine;
using System.Collections;

/// <summary>
/// Player state.
/// </summary>
public class PlayerState : BaseCharacterState {

    /* *** Protected Methods *** */

    /// <summary>
    /// Handle player death.
    /// TODO: We probably don't want to just destroy the instance.
    /// </summary>
    protected override void Die() {
        Destroy(this.gameObject);
    }
}
