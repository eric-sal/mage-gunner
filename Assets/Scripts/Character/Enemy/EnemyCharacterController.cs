using UnityEngine;
using System.Collections;

public class EnemyCharacterController : BaseCharacterController {

    protected EnemyState _enemyState;
    protected PlayerState _playerState;

    public void Start() {
        _enemyState = GetComponent<EnemyState>();
        _playerState = GameObject.Find("Player").GetComponentInChildren<PlayerState>();
    }

    protected override void Act() {
        Aim();

        if (!CanSeePlayer() || _playerState.health <= 0) {
            return;
        }

        var gun = this.equippedFirearm;
        if (gun != null) {
            if (gun.IsEmpty) {
                gun.Reload();
            } else {
                Fire();
            }
        }
    }

    protected override void Aim() {
        _reticle.LerpPosition(_playerState.transform.position, _enemyState.lookSpeed);
    }

    protected bool CanSeePlayer() {

        RaycastHit hitInfo;
        Vector3 direction = _playerState.transform.position - _enemyState.transform.position;

        Debug.DrawRay(_enemyState.transform.position, direction);

        if (Physics.Raycast(_enemyState.transform.position, direction, out hitInfo, 20)) {
            if (hitInfo.collider == _playerState.gameObject.collider) {
                return true;
            }
        }

        return false;
    }
}
