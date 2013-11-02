using UnityEngine;
using System.Collections;

public class Reticle : MonoBehaviour {

    protected Vector3 _recoil;

	// Use this for initialization
	void Start () {
        _recoil = Vector3.zero;
	}
	
    void FixedUpdate () {
        SetPosition(this.transform.position + _recoil);
	}

    public void SetPosition(Vector3 worldPosition) {

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        if (screenPosition.x < 0f) {
            Vector3 leftmostPoint = new Vector3(0f, screenPosition.y, screenPosition.z);
            worldPosition = Camera.main.ScreenToWorldPoint(leftmostPoint);

        } else if (screenPosition.x > Camera.main.pixelWidth) {
            Vector3 rightmostPoint = new Vector3(Camera.main.pixelWidth, screenPosition.y, screenPosition.z);
            worldPosition = Camera.main.ScreenToWorldPoint(rightmostPoint);
        }

        if (screenPosition.y < 0f) {
            Vector3 bottommostPoint = new Vector3(screenPosition.x, 0f, screenPosition.z);
            worldPosition = Camera.main.ScreenToWorldPoint(bottommostPoint);

        } else if (screenPosition.y > Camera.main.pixelHeight) {
            Vector3 topmostPoint = new Vector3(screenPosition.x, Camera.main.pixelHeight, screenPosition.z);
            worldPosition = Camera.main.ScreenToWorldPoint(topmostPoint);
        }

        this.transform.position = worldPosition;
    }

    public void ApplyRecoil(Vector3 recoil) {
        _recoil += recoil;
    }

    public void ReduceRecoil(float amount) {

        float magBefore = _recoil.sqrMagnitude;
        _recoil -= _recoil.normalized * amount;
        float magAfter = _recoil.sqrMagnitude;

        if (magAfter > magBefore) {
            // slid too far in the opposite direction
            _recoil = Vector3.zero;
        }
    }

}
