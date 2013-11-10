using UnityEngine;
using System.Collections;

public class EnemyCharacterController : BaseCharacterController {

    public CharacterState _playerState;

    public void Start() {
        _playerState = GameObject.Find("Player").GetComponentInChildren<CharacterState>();
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
            }
            else {
                Fire();
            }
        }
    }

    protected bool CanSeePlayer() {

        RaycastHit hitInfo;
        Vector3 direction = _playerState.transform.position - _character.transform.position;

        Debug.DrawRay(_character.transform.position, direction);

        if (Physics.Raycast(_character.transform.position, direction, out hitInfo, 20)) {
            if (hitInfo.collider == _playerState.gameObject.collider) {
                return true;
            }
        }

        return false;
    }

    protected override void Aim() {
        _reticle.SetPosition(_playerState.transform.position);
    }
}
