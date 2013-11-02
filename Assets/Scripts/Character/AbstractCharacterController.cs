using UnityEngine;
using System.Collections;

/// <summary>
/// The base class for character handlers.
/// Associated GameObject should have a CharacterState-derived script and a
/// kindred-sprite Sprite-derived script.
///
/// You cannot assign this to a game object directly.  You must inherit
/// from this class.
///
/// Sub-classes should modify character state in the 'Act' function.
/// </summary>
public abstract class AbstractCharacterController : MonoBehaviour {

    protected CharacterState _character;
    protected Sprite _sprite;
    protected MoveableObject _moveable;
    protected ReticleController _reticle;

    public virtual void Awake() {
        _character = GetComponent<CharacterState>();
        _sprite = GetComponent<Sprite>();
        _moveable = GetComponent<MoveableObject>();

        GameObject reticlePrefab = (GameObject)Resources.Load("Prefabs/Reticle");
        Vector3 spawnPosition = _character.transform.position + new Vector3(0, 2);
        GameObject reticleInstance = (GameObject)Instantiate(reticlePrefab, spawnPosition, reticlePrefab.transform.rotation);
        _reticle = reticleInstance.GetComponent<ReticleController>();
    }

    public virtual void Start() {
        _character.position.x = this.transform.position.x;
        _character.position.y = this.transform.position.y;
    }

    /// <summary>
    /// This is to be defined by subclasses.  This method is called from
    /// FixedUpdate before any physics calculations have been performed.
    /// Here would be where player inputs would get recorded and the
    /// character state would get modified.  For enemies, the AI would
    /// alter the state instead.
    /// </summary>
    protected abstract void Act();

    protected abstract void Aim();

    public virtual void FixedUpdate() {
        // Let player or AI modify character state first
        Act();

        if (_moveable != null) {
            _moveable.Move(_character.velocity, Time.deltaTime);
        }

        _reticle.ReduceRecoil(_character.getRecoilReduction());
    }
}
