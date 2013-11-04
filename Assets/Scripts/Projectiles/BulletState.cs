using UnityEngine;
using System.Collections;

public class BulletState : MonoBehaviour {
    public int damage;

    private MoveableObject _moveableObject;

    public void Awake() {
        _moveableObject = GetComponent<MoveableObject>();
    }

    public Vector3 velocity {
        get { return _moveableObject.velocity;  }

        set { _moveableObject.velocity = value; }
    }
}
