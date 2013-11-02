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
        this.transform.position = CameraController.clampPosition(worldPosition);
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
