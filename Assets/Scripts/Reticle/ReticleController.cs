using UnityEngine;
using System.Collections;

public class ReticleController : MonoBehaviour {

    protected Vector3 _recoil;

	// Use this for initialization
	void Start () {
        _recoil = Vector3.zero;
	}
	
    void FixedUpdate () {
        SetPosition(this.transform.position + _recoil);
	}

    public void SetPosition(Vector3 worldPosition) {
        if (SceneController.isPaused) {
            return;
        }

        this.transform.position = CameraController.clampPosition(worldPosition);
    }

    public void LerpTo(Vector3 worldPosition, float speed = 1) {
        if (SceneController.isPaused) {
            return;
        }

        Vector3 position = CameraController.clampPosition(worldPosition);
        this.transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * speed);
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
