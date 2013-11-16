﻿using UnityEngine;
using System.Collections;

public class EnemyCharacterController : BaseCharacterController {

    protected bool _canSeePlayer;
    protected EnemyState _enemyState;
    protected int _layerMask;
    protected PlayerState _playerState;

    protected Vector3 _previousPlayerPosition = Vector3.zero;
    protected Vector3 _estimatedPlayerVelocity;

    public void Start() {
        _enemyState = GetComponent<EnemyState>();
        _playerState = GameObject.Find("Player").GetComponentInChildren<PlayerState>();

        // When checking to see if we can see the player, we want the ray to ignore projectiles.
        _layerMask = ((1 << LayerMask.NameToLayer("Players"))
                    | (1 << LayerMask.NameToLayer("Obstacles"))
                    | (1 << LayerMask.NameToLayer("Enemies")));
    }

    protected override void CaptureInput() {
        FindPlayer();
        EstimatePlayerVelocity();

        if (!_canSeePlayer || _playerState.health <= 0) {
            return;
        }

        Aim();


        var gun = this.equippedFirearm;
        if (gun != null) {
            if (gun.IsEmpty) {
                gun.Reload();
            } else {
                Fire();
            }
        }
    }

    protected override void Act() {
        // No physics calculations for this enemy
    }

    protected override void Aim() {
        Vector3 playerPosition = _playerState.transform.position + _estimatedPlayerVelocity;
        _reticle.LerpPosition(playerPosition, _enemyState.lookSpeed);
    }

    protected void FindPlayer() {
        _canSeePlayer = false;

        RaycastHit hitInfo;
        Vector3 direction = _playerState.transform.position - _enemyState.transform.position;

        Debug.DrawRay(_enemyState.transform.position, direction);

        if (Physics.Raycast(_enemyState.transform.position, direction, out hitInfo, 20, _layerMask)) {
            if (hitInfo.collider == _playerState.gameObject.collider) {
                _canSeePlayer =  true;
            }
        }
    }

    protected void EstimatePlayerVelocity() {
        if (_enemyState.anticipatePlayerMovement && _canSeePlayer) {
            Vector3 currentPlayerPosition = _playerState.transform.position;

            if (_previousPlayerPosition != Vector3.zero) {
                Vector3 deltaPosition = currentPlayerPosition - _previousPlayerPosition;
                _estimatedPlayerVelocity = Vector3.ClampMagnitude(deltaPosition / Time.deltaTime, 1);
            }

            _previousPlayerPosition = currentPlayerPosition;
        } else {
            _previousPlayerPosition = Vector3.zero;
            _estimatedPlayerVelocity = Vector3.zero;
        }
    }
}
