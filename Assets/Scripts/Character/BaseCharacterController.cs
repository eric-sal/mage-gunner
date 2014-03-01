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

    protected Animator _animator;
    protected BaseCharacterState _character;
    protected Inventory _inventory;
    protected ReticleController _reticle;

    public BaseCharacterState character {
        get { return _character; }
    }

    public Inventory inventory {
        get { return _inventory; }
    }

    public ReticleController reticle {
        get { return _reticle; }
    }

    /* *** Constructors *** */
    public virtual void Awake() {
        _animator = GetComponentInChildren<Animator>();
        _character = GetComponent<BaseCharacterState>();
        _inventory = GetComponentInChildren<Inventory>();

        // Create a reticle for this character.
        GameObject reticlePrefab = (GameObject)Resources.Load("Prefabs/Reticle");
        Vector2 spawnPosition = (Vector2)_character.transform.position + _character.lookDirection;
        GameObject reticleInstance = (GameObject)Instantiate(reticlePrefab, spawnPosition, reticlePrefab.transform.rotation);
        reticleInstance.transform.parent = _character.transform;
        _reticle = reticleInstance.GetComponent<ReticleController>();
        _character.LookAt(_reticle.transform.position);
    }


    /* *** MonoBehaviour Methods *** */

    public virtual void Update() {
        _character.LookAt(_reticle.transform.position);

        bool isWalking = _character.velocity != Vector2.zero;
        _animator.SetBool("walking", isWalking);
        
        Vector2 direction = _character.lookDirection.normalized;
        Vector3 localScale = _animator.transform.localScale;
        localScale.x = direction.x < 0 ? -1 : 1;
        _animator.transform.localScale = localScale;
        
        _animator.SetFloat("inputX", Mathf.Abs(direction.x));
        _animator.SetFloat("inputY", direction.y);
    }

    public virtual void FixedUpdate() { }

    /* *** Member Methods *** */

    /// <summary>
    /// Equip the passed in firearm
    /// </summary>
    /// <param name='firearm'>
    /// The firearm to equip.
    /// </param>
    public void EquipWeapon(Firearm firearm) {
        _character.equippedFirearm = firearm;
        _reticle.ResetRecoil();
        _reticle.updateReticlePosition = firearm.recoilMovesReticle;
    }

    /// <summary>
    /// Fires the currently equipped firearm, and applies the recoil
    /// to the reticle.
    /// </summary>
    public void Fire() {
        _Fire();
    }

    protected void _Fire() {
        Vector2 bulletVector = _reticle.OffsetPosition - (Vector2)this.transform.position;
        Vector2 recoil = _character.equippedFirearm.Fire(bulletVector);

        // TODO: Extract into a generic utility function. Similar functionality to ReduceRecoil.
        float magBefore = recoil.sqrMagnitude;
        recoil -= recoil.normalized * (_character.strength / 2);
        float magAfter = recoil.sqrMagnitude;

        if (magAfter > magBefore) {
            // slid too far in the opposite direction
            recoil = Vector2.zero;
        }

        _reticle.AddRecoil(recoil);
    }
}
