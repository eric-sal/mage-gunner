using UnityEngine;
using System.Collections;

public class EnemyState : BaseCharacterState {
	public int lookSpeed = 2;
    public bool anticipatePlayerMovement = true;    // Mostly for debugging. Can probably remove this later.
}
