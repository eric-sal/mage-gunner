using UnityEngine;
using System.Collections;

public class CoverController : MonoBehaviour {
    protected bool _active = false;
    protected Renderer _renderer;
    
    public void Awake() {
        _renderer = GetComponent<Renderer>();
    }

    public bool isActive {
        get { return _active; }
    }

    public void Activate() {
        _active = true;
        _renderer.material.SetColor("_Color", Color.cyan);
    }

    public void Deactivate() {
        _active = false;
        _renderer.material.SetColor("_Color", Color.red);
    }
}
