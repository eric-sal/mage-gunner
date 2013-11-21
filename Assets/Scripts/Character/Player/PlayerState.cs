using UnityEngine;
using System.Collections;

/// <summary>
/// Player state.
/// </summary>
public class PlayerState : BaseCharacterState {

    /* *** Member Methods *** */

    /// <summary>
    /// Handle player death.
    /// TODO: We probably don't want to just destroy the instance.
    /// </summary>
    protected override void _Die() {
        Destroy(this.gameObject);
    }
}
