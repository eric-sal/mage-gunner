using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoverCollisionSensorController : MonoBehaviour {
    public CoverCollisionSensor neSensor;
    public CoverCollisionSensor nwSensor;
    public CoverCollisionSensor seSensor;
    public CoverCollisionSensor swSensor;
    
    private BaseCharacterController _controller;
    private bool _wasInCover = false;
    
    void Awake() {
        _controller = this.transform.parent.GetComponent<BaseCharacterController>();
    }
	
    void Update() {
        if (_inCover()) {
            if (!_wasInCover) {
                _controller.Kneel();
                this._activateCover();
            }

            _wasInCover = true;
        } else if (_wasInCover) {
            _controller.Stand();
            _wasInCover = false;
        }
    }

    private void _activateCover() {
        List<CoverController> covers = new List<CoverController>();
        covers.AddRange(neSensor.Covers);
        covers.AddRange(nwSensor.Covers);
        covers.AddRange(seSensor.Covers);
        covers.AddRange(swSensor.Covers);

        foreach (CoverController cover in covers) {
            cover.Activate();
        }
    }

    private bool _collidedNorth() {
        return neSensor.Triggered && nwSensor.Triggered;
    }

    private bool _collidedSouth() {
        return seSensor.Triggered && swSensor.Triggered;
    }
    
    private bool _collidedEast() {
        return neSensor.Triggered && seSensor.Triggered;
    }
    
    private bool _collidedWest() {
        return nwSensor.Triggered && swSensor.Triggered;
    }
    
    private bool _inCover() {
        return this._collidedNorth() || this._collidedSouth() || this._collidedEast() || this._collidedWest();
    }
}
