using UnityEngine;
using System;
using System.Collections;

public class Firearm : MonoBehaviour {

    public enum FireType { Standard, Spray, Spread, Burst }

    public int ammoConsumed = 1;    // How many rounds does this use up when fired? Burst fire weapons and double-barreled shotguns would have a value > 1.
    public AudioSource audioSource;
    public float bulletSpeed;
    public FireType fireType;
    public bool fullAuto;
    public int magazineSize;
    public int maxDamage = 1;   // per projectile
    public int minDamage = 1;   // per projectile
    public int numProjectiles = 1;  // How many bullets do we spawn when the trigger is pulled? Defaults to 1. Different than ammoConsumed. Buckshot spawns many projectiles but consumes 1 round.
    public float rateOfFire;
    public float recoil;
    public float scatterVariation;  // In degrees. Closer to 0 is less scatter.

    private float _cycleTime; // Time per bullet (inverse of rate of fire).
    private float _elapsed;
    private int _roundsFired;

    private static GameObject _bulletBucket;
    private static GameObject _bulletPrefab;

	void Start() {
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

        _cycleTime = 60 / rateOfFire;
        _elapsed = _cycleTime; // so we can shoot right away
	}

    void Update() {
        _elapsed += Time.deltaTime;
    }

    public float randomRecoil {
        get { return UnityEngine.Random.Range(-recoil, recoil); }
    }

    /// <summary>
    /// Fire the specified direction.
    /// Returns a Vector3 representing the degree of recoil.
    /// </summary>
    /// <param name='direction'>
    /// The direction we're firing in.
    /// </param>
	public Vector3 Fire(Vector3 direction) {
        if (_elapsed <= _cycleTime || _roundsFired >= magazineSize) {
            return Vector3.zero;
        }

        _elapsed = 0;

        switch (fireType) {
        case FireType.Standard:
            FireStandardShot(direction);
            break;
        case FireType.Spray:
            FireSprayShot(direction);
            break;
        case FireType.Spread:
            FireSpreadShot(direction);
            break;
        case FireType.Burst:
            StartCoroutine("FireBurstShot", direction);
            break;
        }

        _roundsFired += ammoConsumed;

        return new Vector3(randomRecoil, randomRecoil);
    }

    public void Reload() {
        // We can't reload while we're firing.
        if (_elapsed > _cycleTime) {
            _roundsFired = 0;
        }
    }

    // TODO: Maybe accuracy can be affected by the player's proficiency?
    // So a shot fired in a specific direction won't necessarily travel *exactly* in that
    // direction if the player isn't proficient enough in the weapon.
    // Standard gun fire.
    // ex: pistol, SMG, assault rifle
    private void FireStandardShot(Vector3 direction) {
        for (int i = 0; i < ammoConsumed; i ++) {
            SpawnBullet(direction);
        }
    }

    // Buckshot-like spray.
    // ex: shotgun
    private void FireSprayShot(Vector3 direction) {
        float scatterAmount;
        Quaternion quato = Quaternion.LookRotation(direction, Vector3.forward);

        for (int i = 0; i < numProjectiles; i++) {
            scatterAmount = UnityEngine.Random.Range(-scatterVariation, scatterVariation);
            SpawnBullet(quato * Quaternion.Euler(0, scatterAmount, 0) * Vector3.forward);
        }
    }

    // A spread shot.
    // ex: the "Spread" gun from Contra.
    private void FireSpreadShot(Vector3 direction) {
        // If we weren't in a top-down (x, y plane) view, I *think* we would use a
        // different vector than Vector3.forward <0, 0, 1> as the 2nd param here.
        Quaternion quato = Quaternion.LookRotation(direction, Vector3.forward);

        for (int i = (int)Mathf.Floor(numProjectiles / 2f) * -1; i < (int)Mathf.Ceil(numProjectiles / 2f); i++) {
            SpawnBullet(quato * Quaternion.Euler(0, scatterVariation * i, 0) * Vector3.forward);
        }
    }

    // Fires 3 bullets in one pull of the trigger.
    // Includes a bit of a recoil effect.
    private IEnumerator FireBurstShot(Vector3 direction) {
        Quaternion quato = Quaternion.LookRotation(direction, Vector3.forward);

        SpawnBullet(direction);
        for (int i = 0; i < ammoConsumed - 1; i++) {
            yield return new WaitForSeconds(_cycleTime);

            _elapsed = 0;   // Set _elapsed to 0 so we can't fire the weapon again while this coroutine is running.
            SpawnBullet(quato * Quaternion.Euler(0, scatterVariation, 0) * Vector3.forward);
        }
    }

    private void SpawnBullet(Vector3 direction) {
        GameObject bullet = (GameObject)Instantiate(_bulletPrefab, this.transform.position, _bulletPrefab.transform.rotation);
        bullet.transform.parent = _bulletBucket.transform;

        BulletState bulletState = bullet.GetComponent<BulletState>();
        bulletState.velocity = Vector3.ClampMagnitude(direction, 1) * this.bulletSpeed;
        bulletState.damage = RollForDamage();
    }

    private int RollForDamage() {
        return Mathf.RoundToInt(UnityEngine.Random.Range(minDamage, maxDamage));
    }
}
