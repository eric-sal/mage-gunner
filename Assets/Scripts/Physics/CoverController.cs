using UnityEngine;
using System.Collections;

public class CoverController : MonoBehaviour {
    private bool _active = false;

    public bool isActive {
        get { return _active; }
    }

    public void Activate() {
        _active = true;
        this.renderer.material.SetColor("_Color", Color.cyan);
    }

    public void Deactivate() {
        _active = false;
        this.renderer.material.SetColor("_Color", Color.red);
    }
}
