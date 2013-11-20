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

    public Firearm equippedFirearm;

    protected BaseCharacterState _character;
    protected Inventory _inventory;
    protected MoveableObject _moveable;
    protected ReticleController _reticle;

    /* *** Constructors *** */

    public virtual void Awake() {
        _character = GetComponent<BaseCharacterState>();
        _inventory = this.transform.parent.gameObject.GetComponentInChildren<Inventory>();
        _moveable = GetComponent<MoveableObject>();

        // Create a reticle for this character.
        GameObject reticlePrefab = (GameObject)Resources.Load("Prefabs/Reticle");
        Vector3 spawnPosition = _character.transform.position + new Vector3(0, 2);
        GameObject reticleInstance = (GameObject)Instantiate(reticlePrefab, spawnPosition, reticlePrefab.transform.rotation);
        reticleInstance.transform.parent = _character.transform;
        _reticle = reticleInstance.GetComponent<ReticleController>();
    }

    /* *** MonoBehaviour Methods *** */

    public virtual void Update() {
        CaptureInput();
    }

    public virtual void FixedUpdate() {
        Act(); // Let player or AI modify character state first

        if (_moveable != null) {
            _moveable.Move(Time.deltaTime);
        }

        _reticle.ReduceRecoil(_character.GetRecoilReduction());
    }

    /* *** Protected Methods *** */

    /// <summary>
    /// This is to be defined by subclasses. This method is called
    /// from Update. It should capture character input.
    /// </summary>
    protected abstract void CaptureInput();

    /// <summary>
    /// This is to be defined by subclasses. This method is called
    /// from FixedUpdate before any physics calculations have been
    /// performed. Here would be where player inputs would get
    /// recorded and the character state would get modified.  For
    /// enemies, the AI would alter the state instead.
    /// </summary>
    protected abstract void Act();

    /// <summary>
    /// Define in subclasses. Handles aiming the character's reticle.
    /// </summary>
    protected abstract void Aim();

    /// <summary>
    /// Fires the currently equipped firearm, and applies the recoil
    /// to the reticle.
    /// </summary>
    protected void Fire() {
        Vector3 bulletVector = _reticle.transform.position - this.transform.position;
        Vector3 recoil = equippedFirearm.Fire(bulletVector);
        _reticle.ApplyRecoil(recoil);
    }
}
