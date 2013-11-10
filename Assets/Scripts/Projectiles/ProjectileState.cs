using UnityEngine;
using System.Collections;

public class ProjectileState : MonoBehaviour {
    public int damage;
    public GameObject spawner;  // GameObject that spawned the projectile

    private MoveableObject _moveableObject;

    public void Awake() {
        _moveableObject = GetComponent<MoveableObject>();
    }

    public Vector3 velocity {
        get { return _moveableObject.velocity;  }

        set { _moveableObject.velocity = value; }
    }
}
