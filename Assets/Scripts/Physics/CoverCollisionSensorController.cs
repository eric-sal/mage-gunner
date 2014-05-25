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
            _wasInCover = true;
            _controller.Kneel();
        } else if (_wasInCover) {
            _wasInCover = false;
            _controller.Stand();
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
        return _collidedNorth() || _collidedSouth() || _collidedEast() || _collidedWest();
    }
}
