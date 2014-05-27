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
    private bool _popUp = false;
    
    void Awake() {
        _controller = this.transform.parent.GetComponent<BaseCharacterController>();
    }
	
    void Update() {
        if (_inCover() && !_popUp) {
            _controller.Kneel();
        } else {
            _controller.Stand();
        }

        _popUp = false;
    }

    private void _activateCover(IEnumerable<CoverController> covers) {
        foreach (CoverController cover in covers) {
            cover.Activate();
        }
    }

    private void _deactivateCover(IEnumerable<CoverController> covers) {
        foreach (CoverController cover in covers) {
            cover.Deactivate();
        }
    }

    private bool _checkCollisionNorth() {
        bool triggered = neSensor.Triggered && nwSensor.Triggered;
        if (triggered) {
            _popUp = _popUp || Input.GetAxis("Vertical") >= 0.5f;

            var cover = neSensor.Covers.Union(nwSensor.Covers);
            if (_popUp) {
                this._deactivateCover(cover);
            } else {
                this._activateCover(cover);
            }
        }

        return triggered;
    }

    private bool _checkCollisionSouth() {
        bool triggered = seSensor.Triggered && swSensor.Triggered;
        if (triggered) {
            _popUp = _popUp || Input.GetAxis("Vertical") <= -0.5f;

            var cover = seSensor.Covers.Union(swSensor.Covers);
            if (_popUp) {
                this._deactivateCover(cover);
            } else {
                this._activateCover(cover);
            }
        }

        return triggered;
    }

    private bool _checkCollisionEast() {
        bool triggered = neSensor.Triggered && seSensor.Triggered;
        if (triggered) {
            _popUp = _popUp || Input.GetAxis("Horizontal") >= 0.5f;

            var cover = neSensor.Covers.Union(seSensor.Covers);
            if (_popUp) {
                this._deactivateCover(cover);
            } else {
                this._activateCover(cover);
            }
        }

        return triggered;
    }

    private bool _checkCollisionWest() {
        bool triggered = nwSensor.Triggered && swSensor.Triggered;
        if (triggered) {
            _popUp = _popUp || Input.GetAxis("Horizontal") <= -0.5f;

            var cover = nwSensor.Covers.Union(swSensor.Covers);
            if (_popUp) {
                this._deactivateCover(cover);
            } else {
                this._activateCover(cover);
            }
        }

        return triggered;
    }

    private bool _inCover() {
        // We'll call these individually, because we want each one of these
        // methods to execute completely. Calling them chained in a conditional
        // would short-circuit the statement, preventing the other methods
        // from executing as soon as one returns true.
        bool collidedNorth = this._checkCollisionNorth();
        bool collidedSouth = this._checkCollisionSouth();
        bool collidedEast = this._checkCollisionEast();
        bool collidedWest = this._checkCollisionWest();

        return collidedNorth || collidedSouth || collidedEast || collidedWest;
    }
}
