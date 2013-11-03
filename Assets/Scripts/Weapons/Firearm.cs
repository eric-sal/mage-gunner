using UnityEngine;
using System;
using System.Collections;

public class Firearm : MonoBehaviour {

    public enum Scatter { Standard, Spray, Spread }

    public Scatter scatter;
    public float scatterVariation;  // Closer to 0 is less scatter. In degrees for Scatter.Spread (& eventually Scatter.Spray).
    public int numProjectiles = 1; // How many bullets do we spawn when the trigger is pulled? Defaults to 1.
    public float rateOfFire;
    public int magazineSize;
    public float recoil;
    public bool fullAuto;
    public float bulletSpeed;
    public AudioSource audioSource;

    private float _cycleTime; // Time per bullet (inverse of rate of fire).
    private float _elapsed;
    private int _roundsFired;

    private static GameObject _bulletBucket;
    private static GameObject _bulletPrefab;

	void Start() {
        if (rateOfFire <= 0) {
            throw new InvalidOperationException("rateOfFire must be >= 0!");
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

        switch (scatter) {
        case Scatter.Standard:
            fireStandardScatter(direction);
            break;
        case Scatter.Spray:
            fireSprayScatter(direction);
            break;
        case Scatter.Spread:
            fireSpreadScatter(direction);
            break;
        }

        _roundsFired += 1;

        return new Vector3(randomRecoil, randomRecoil);
    }

    public void Reload() {
        _roundsFired = 0;
    }

    // TODO: Maybe accuracy can be affected by the player's proficiency?
    // So a shot fired in a specific direction won't necessarily travel *exactly* in that
    // direction if the player isn't proficient enough in the weapon.
    // Standard gun fire.
    // ex: pistol, SMG, assault rifle
    private void fireStandardScatter(Vector3 direction) {
        spawnBullet(direction);
    }

    // TODO: Should mimic the fireSpreadScatter method, except the offset should be randomized
    // within +/- scatterVariation.
    // Buckshot-like spray.
    // ex: shotgun
    private void fireSprayScatter(Vector3 direction) {
        float offsetAmount;
        Vector3 offset;

        for (int i = 0; i < numProjectiles; i++) {
            offsetAmount = UnityEngine.Random.Range(-scatterVariation, scatterVariation);
            offset = new Vector3(offsetAmount, offsetAmount);
            spawnBullet(direction - offset);
        }
    }

    // TODO: Make this work right...
    // The bullets should be spawned scatterVariation degrees apart with an equal
    // number of projectiles on either side of the direction vector.
    // A spread shot.
    // ex: the "Spread" gun from Contra.
    private void fireSpreadScatter(Vector3 direction) {
        float radians;
        Vector3 offset;

        // Using Vector3.right isn't right, but this is on the right track.
        float directionAngle = Vector3.Angle(direction, Vector3.right) * Mathf.Deg2Rad;

        Debug.Log(string.Format("Direction: {0}, {1}", direction.x, direction.y));
        for (int i = (int)Mathf.Floor(numProjectiles / 2f) * -1; i < (int)Mathf.Ceil(numProjectiles / 2f); i++) {
            radians = scatterVariation * i * Mathf.Deg2Rad;

            // Inspiration: http://answers.unity3d.com/questions/170413/position-objects-around-other-object-forming-a-cir.html
            offset = new Vector3(Mathf.Sin(directionAngle + radians), Mathf.Cos(directionAngle + radians));

            Debug.Log(string.Format("New Direction: {0}, {1}", offset.x, offset.y));
            spawnBullet(offset);
        }
    }

    private void spawnBullet(Vector3 direction) {
        GameObject bullet = (GameObject)Instantiate(_bulletPrefab, this.transform.position, _bulletPrefab.transform.rotation);
        bullet.transform.parent = _bulletBucket.transform;

        MoveableObject bulletState = bullet.GetComponent<MoveableObject>();
        bulletState.velocity = Vector3.ClampMagnitude(direction, 1) * this.bulletSpeed;
    }
}
