using UnityEngine;
using System;
using System.Collections;

public class Firearm : MonoBehaviour {

    public float rateOfFire;    // rateOfFire should be set by the subclass
    public int magazineSize;
    public float recoil;
    public float bulletSpeed;
    public AudioSource audioSource;

    private float _cycleTime; // time per bullet (inverse of rate of fire)
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

        var angle = Vector3.Angle(Vector3.right, direction);
        float x = Mathf.Cos(angle / 180 * Mathf.PI) * this.bulletSpeed;
        float y = Mathf.Sin(angle / 180 * Mathf.PI) * this.bulletSpeed;

        if (direction.y < 0) {
            // shooting down
            y *= -1;
        }

        GameObject bullet = (GameObject)Instantiate(_bulletPrefab, this.transform.position, _bulletPrefab.transform.rotation);
        bullet.transform.parent = _bulletBucket.transform;
        var bulletState = bullet.GetComponent<MoveableObject>();
        bulletState.velocity = new Vector3(x, y, 0);
        _roundsFired += 1;

        return new Vector3(randomRecoil, randomRecoil);
    }

    public void Reload() {
        _roundsFired = 0;
    }
}
