using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
            _controller.Kneel();
            _wasInCover = true;
        } else if (_wasInCover) {
            _controller.Stand();
            _wasInCover = false;
        }
    }

    private void _activateCover(IEnumerable<CoverController> covers) {
        foreach (CoverController cover in covers) {
            cover.Activate();
        }
    }

    private bool _collidedNorth() {
        bool triggered = neSensor.Triggered && nwSensor.Triggered;
        if (triggered) {
            this._activateCover(neSensor.Covers.Union(nwSensor.Covers));
        }

        return triggered;
    }

    private bool _collidedSouth() {
        bool triggered = seSensor.Triggered && swSensor.Triggered;
        if (triggered) {
            this._activateCover(seSensor.Covers.Union(swSensor.Covers));
        }

        return triggered;
    }
    
    private bool _collidedEast() {
        bool triggered = neSensor.Triggered && seSensor.Triggered;
        if (triggered) {
            this._activateCover(neSensor.Covers.Union(seSensor.Covers));
        }

        return triggered;
    }
    
    private bool _collidedWest() {
        bool triggered = nwSensor.Triggered && swSensor.Triggered;
        if (triggered) {
            this._activateCover(nwSensor.Covers.Union(swSensor.Covers));
        }

        return triggered;
    }
    
    private bool _inCover() {
        return this._collidedNorth() || this._collidedSouth() || this._collidedEast() || this._collidedWest();
    }
}
