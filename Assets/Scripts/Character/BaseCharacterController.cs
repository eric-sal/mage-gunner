using UnityEngine;
using System.Collections;

/// <summary>
/// The base class for character handlers.
/// Associated GameObject should have a CharacterState-derived script
/// and a kindred-sprite Sprite-derived script.
///
/// You cannot assign this to a game object directly.  You must
/// inherit from this class.
///
/// Sub-classes should modify character state in the 'Act' function.
/// </summary>
public abstract class BaseCharacterController : MonoBehaviour {

    /* *** Member Variables *** */

    protected BaseCharacterState _character;
    protected Inventory _inventory;
    protected MoveableObject _moveable;
    protected ReticleController _reticle;

    public BaseCharacterState character {
        get { return _character; }
    }

    public Inventory inventory {
        get { return _inventory; }
    }

    public MoveableObject moveable {
        get { return _moveable; }
    }

    public ReticleController reticle {
        get { return _reticle; }
    }

    /* *** Constructors *** */

    public virtual void Awake() {
        _character = GetComponent<BaseCharacterState>();
        _inventory = GetComponentInChildren<Inventory>();
        _moveable = GetComponent<MoveableObject>();

        // Create a reticle for this character.
        GameObject reticlePrefab = (GameObject)Resources.Load("Prefabs/Reticle");
        Vector3 spawnPosition = _character.transform.position + _character.lookDirection;
        GameObject reticleInstance = (GameObject)Instantiate(reticlePrefab, spawnPosition, reticlePrefab.transform.rotation);
        reticleInstance.transform.parent = _character.transform;
        _reticle = reticleInstance.GetComponent<ReticleController>();
        _character.LookAt(_reticle.transform.position);
    }

    /* *** MonoBehaviour Methods *** */

    public virtual void Update() {
        _character.LookAt(_reticle.transform.position);
    }

    public virtual void FixedUpdate() {
        if (_moveable != null) {
            _moveable.Move(Time.deltaTime);
        }

        _reticle.ReduceRecoil(_character.GetRecoilReduction());
    }

    /* *** Member Methods *** */

    /// <summary>
    /// Fires the currently equipped firearm, and applies the recoil
    /// to the reticle.
    /// </summary>
    public void Fire() {
        _Fire();
    }

    protected void _Fire() {
        Vector3 bulletVector = _reticle.transform.position - this.transform.position;
        Vector3 recoil = _character.equippedFirearm.Fire(bulletVector);
        _reticle.ApplyRecoil(recoil);
    }
}
