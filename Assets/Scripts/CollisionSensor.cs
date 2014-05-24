using UnityEngine;
using System.Collections;

public class CollisionSensor : MonoBehaviour {

    public void OnTriggerStay(Collider c) {
        if (c.gameObject.name == "Player") {
            return;
        }
        Debug.Log("I am colliding with " + c.gameObject.name);
    }

    public void OnTriggerEnter(Collider c) {
        if (c.gameObject.name == "Player") {
            return;
        }
        var scale = this.transform.parent.transform.localScale;
        var halfHeight = new Vector3(scale.x, scale.y, scale.z / 2);
        this.transform.parent.transform.localScale = halfHeight;

        var position = this.transform.parent.position;
        var newPosition = new Vector3(position.x, position.y, position.z + halfHeight.z / 2);
        this.transform.parent.transform.position = newPosition;
    }

    public void OnTriggerExit(Collider c) {
        if (c.gameObject.name == "Player") {
            return;
        }
        var scale = this.transform.parent.transform.localScale;
        var fullHeight = new Vector3(scale.x, scale.y, scale.z * 2);
        this.transform.parent.transform.localScale = fullHeight;

        var position = this.transform.parent.position;
        var newPosition = new Vector3(position.x, position.y, position.z - fullHeight.z / 4);
        this.transform.parent.transform.position = newPosition;
    }
}
