using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Customizable firearm class. Add as a component to a firearm GameObject.
/// </summary>
/// <exception cref='InvalidOperationException'>
/// Thrown when rateOfFire <= 0 or when minDamage < maxDamage.
/// </exception>
public class Firearm : MonoBehaviour {

    public enum FireType {
        Standard,
        Spray,
        Spread,
        Burst
    }

    /* *** Class Variables *** */

    private static GameObject _bulletBucket;    // Empty GameObject for keeping spawned bullets.
    private static GameObject _bulletPrefab;

    /* *** Member Variables *** */

    public int ammoConsumed = 1;    // How many rounds does this use up when fired? Burst fire weapons and double-barreled shotguns would have a value > 1.
    public float bulletVelocity;   // The velocity with which the bullet leaves the firearm (Unity caps the max value at 100)
    public AudioClip dryFireSound; // The sound clip to play when firing but the magazine is empty
    public FireType fireType;
    public AudioClip[] firingSounds; // Different sound clips to play when firing a bullet from this firearm
    public bool fullAuto;       // Does this weapon fire in full auto mode?
    public int magazineSize;    // The number of rounds the weapon holds per magazine.
    public int maxDamage = 1;   // per projectile
    public int minDamage = 1;   // per projectile
    public int numProjectiles = 1;  // How many bullets do we spawn when the trigger is pulled? Defaults to 1. Different than ammoConsumed. Buckshot spawns many projectiles but consumes 1 round.
    public float rateOfFire;        // The number of rounds per minute.
    public float recoil;            // Closer to 0 is less recoil.
    public bool recoilMovesReticle; // When firing, the recoil from this weapon updates the transform of the reticle.
    public AudioClip reloadSound; // The sound clip to play when reloading
    public float scatterVariation;  // In degrees. Closer to 0 is less scatter.

    private AudioSource _audioSource;
    private BaseCharacterState _character;
    private float _cycleTime; // Time per bullet (inverse of rate of fire).
    private float _elapsed;   // How much time has elapsed since the last time the weapon was fired.
    private int _roundsFired; // The number of rounds the weapon has fired so far.

    /* *** Properties *** */

    public bool IsEmpty {
        get { return _roundsFired >= this.magazineSize; }
    }

    /* *** Constructors *** */

    public void Awake() {
        if (rateOfFire <= 0) {
            throw new InvalidOperationException("rateOfFire must be > 0!");
        }

        if (minDamage > maxDamage) {
            throw new InvalidOperationException("minDamage must be < maxDamage!");
        }

        if (_bulletPrefab == null) {
            _bulletPrefab = (GameObject)Resources.Load("Prefabs/Bullet");
        }

        if (_bulletBucket == null) {
            _bulletBucket = GameObject.Find("BulletBucket");
        }

        _audioSource = GetComponent<AudioSource>();
        
        _cycleTime = 60 / this.rateOfFire;
        _elapsed = _cycleTime; // so we can shoot right away
    }

    public void Start() {
        // Belongs here instead of Awake, because the object is instantiated dynamically,
        // and the parent is set after instantiation.
        _character = this.transform.parent.parent.GetComponent<BaseCharacterState>();
    }

    /* *** MonoBehaviour Methods *** */

    public void Update() {
        _elapsed += Time.deltaTime;
    }

    /* *** Member Methods *** */

    /// <summary>
    /// Generates a random vector representing an offset amount to move the character's reticle.
    /// </summary>
    /// <returns>
    /// A Vector3.
    /// </returns>
    public Vector3 RandomRecoil() {
        float x = UnityEngine.Random.Range(-1f, 1f);
        float z = UnityEngine.Random.Range(-1f, 1f);
        var direction = new Vector3(x, 0f, z).normalized;
        float magnitude = (this.recoil + 1) * 2;
        return direction * magnitude;
    }

    /// <summary>
    /// Fire the specified direction.
    /// Returns a Vector3 representing the degree of recoil.
    /// </summary>
    /// <param name='direction'>
    /// The direction in which we're firing.
    /// </param>
    public Vector3 Fire(Vector3 direction) {
        
        if (_elapsed <= _cycleTime) {
            return Vector3.zero;
        }
        _elapsed = 0;
        
        if (this.IsEmpty) {
            if (this.dryFireSound != null) {
                _audioSource.PlayOneShot(this.dryFireSound);
            }

            return Vector3.zero;
        }

        switch (this.fireType) {
        case FireType.Standard:
            _FireStandardShot(direction);
            break;
        case FireType.Spray:
            _FireSprayShot(direction);
            break;
        case FireType.Spread:
            _FireSpreadShot(direction);
            break;
        case FireType.Burst:
            StartCoroutine(_FireBurstShot(direction));
            break;
        }

        if (this.firingSounds.Length > 0) {
            _audioSource.PlayOneShot(_GetRandomFireSound());
        }
        
        _roundsFired += this.ammoConsumed;

        return RandomRecoil();
    }
    
    private AudioClip _GetRandomFireSound() {
        int index = UnityEngine.Random.Range(0, this.firingSounds.Length - 1);
        return this.firingSounds[index];
    }

    /// <summary>
    /// Reload the weapon.
    /// </summary>
    public void Reload() {
        // We can't reload while we're firing.
        if (_elapsed > _cycleTime && _roundsFired > 0) {
            _roundsFired = 0;

            if (this.reloadSound != null) {
                _audioSource.PlayOneShot(this.reloadSound);
            }
        }
    }

    /// <summary>
    /// Fires the standard shot.
    /// TODO: Maybe accuracy can be affected by the player's proficiency?
    /// So a shot fired in a specific direction won't necessarily travel *exactly* in that
    /// direction if the player isn't proficient enough in the weapon.
    /// Standard gun fire.
    /// ex: pistol, SMG, assault rifle
    /// </summary>
    /// <param name='direction'>
    /// The direction in which we're firing.
    /// </param>
    private void _FireStandardShot(Vector3 direction) {
        for (int i = 0; i < this.ammoConsumed; i ++) {
            _SpawnBullet(direction);
        }
    }

    /// <summary>
    /// Fires the spray shot.
    /// Buckshot-like spray.
    /// ex: shotgun
    /// </summary>
    /// <param name='direction'>
    /// The direction in which we're firing.
    /// </param>
    private void _FireSprayShot(Vector3 direction) {
        float scatterAmount;
        Quaternion quato = Quaternion.LookRotation(direction, Vector3.up);

        for (int i = 0; i < this.numProjectiles; i++) {
            scatterAmount = UnityEngine.Random.Range(-scatterVariation, scatterVariation);
            _SpawnBullet(quato * Quaternion.Euler(0, 0, scatterAmount) * Vector3.up);
        }
    }

    /// <summary>
    /// Fires the spread shot.
    /// A spread shot.
    /// ex: the "Spread" gun from Contra.
    /// </summary>
    /// <param name='direction'>
    /// The direction in which we're firing.
    /// </param>
    private void _FireSpreadShot(Vector3 direction) {
        // If we weren't in a top-down (x, y plane) view, I *think* we would use a
        // different vector than Vector3.forward <0, 0, 1> as the 2nd param here.
        Quaternion quato = Quaternion.LookRotation(direction, Vector3.up);

        for (int i = (int)Mathf.Floor(this.numProjectiles / 2f) * -1; i < (int)Mathf.Ceil(this.numProjectiles / 2f); i++) {
            _SpawnBullet(quato * Quaternion.Euler(0, 0, scatterVariation * i) * Vector3.up);
        }
    }

    /// <summary>
    /// Fires the burst shot.
    /// Fires 3 bullets in one pull of the trigger.
    /// Includes a bit of a recoil effect per round.
    /// </summary>
    /// <param name='direction'>
    /// The direction in which we're firing.
    /// </param>
    private IEnumerator _FireBurstShot(Vector3 direction) {
        float scatterAmount;
        Quaternion quato = Quaternion.LookRotation(direction, Vector3.up);

        _SpawnBullet(direction);
        for (int i = 0; i < this.ammoConsumed - 1; i++) {
            yield return new WaitForSeconds(_cycleTime);

            _elapsed = 0;   // Set _elapsed to 0 so we can't fire the weapon again while this coroutine is running.
            scatterAmount = UnityEngine.Random.Range(-scatterVariation, scatterVariation);
            _SpawnBullet(quato * Quaternion.Euler(0, 0, scatterAmount) * Vector3.up);
        }
    }

    /// <summary>
    /// Spawns a bullet.
    /// </summary>
    /// <param name='direction'>
    /// The direction in which we're firing.
    /// </param>
    private void _SpawnBullet(Vector3 direction) {
        // _character.transform.position isn't quite right. Might want to figure
        // out where the character will be in the NEXT frame and use that position.
        Vector3 ap = _character.aimPoint;
        Vector3 bulletOrigin = new Vector3(ap.x, ap.y + 0.1f, ap.z); // TODO: Adjust for bullet drop due to gravity instead of hardcoding 0.1
        GameObject bullet = (GameObject)Instantiate(_bulletPrefab, bulletOrigin, _bulletPrefab.transform.rotation);
        bullet.transform.parent = _bulletBucket.transform;

        ProjectileState bulletState = bullet.GetComponent<ProjectileState>();
        bulletState.spawner = _character.gameObject;
        bulletState.damage = _RollForDamage();
        //Debug.DrawRay(_character.aimPoint, direction, Color.yellow, 0.5f);
        bulletState.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(direction, 1) * this.bulletVelocity;
    }

    /// <summary>
    /// Picks a random number between minDamage and maxDamage.
    /// </summary>
    /// <returns>
    /// The amount of damage the bullet should do.
    /// </returns>
    private int _RollForDamage() {
        return Mathf.RoundToInt(UnityEngine.Random.Range(this.minDamage, this.maxDamage));
    }
}
