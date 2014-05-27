using UnityEngine;
using System.Collections;

public class CoverController : MonoBehaviour {
    private bool _active = false;
    private bool _wasActive = false;  // So we're not setting the color every frame in which the cover is !_active.

    void Update() {
        if (_active) {
            _wasActive = true;
            this.renderer.material.SetColor("_Color", Color.cyan);
        } else if (_wasActive) {
            _wasActive = false;
            this.renderer.material.SetColor("_Color", Color.red);
        }
    }

    public void Activate() {
        _active = true;
    }

    public void Deactivate() {
        _active = false;
    }
}
