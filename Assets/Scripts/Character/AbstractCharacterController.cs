using UnityEngine;
using System.Collections;

/// <summary>
/// Updates character position based on velocity and handles collisions.
/// Associated GameObject should have a Character-derived script and a
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
    protected Transform _transform;
    protected float _colliderBoundsOffsetX;
    protected float _colliderBoundsOffsetY;
    protected float _skinThickness;
    protected AbstractCollisionHandler _collisionHandler;

    public virtual void Awake() {
        _character = GetComponent<CharacterState>();
        _sprite = GetComponent<Sprite>();
        _transform = this.transform;
        _collisionHandler = GetComponent<AbstractCollisionHandler>();
    }

    public virtual void Start() {
        _character.position.x = _transform.position.x;
        _character.position.y = _transform.position.y;

        _colliderBoundsOffsetX = this.collider.bounds.extents.x;
        _colliderBoundsOffsetY = this.collider.bounds.extents.y;
        _skinThickness = 0.01f;
    }

    /// <summary>
    /// This is to be defined by subclasses.  This method is called from
    /// FixedUpdate before any physics calculations have been performed.
    /// Here would be where player inputs would get recorded and the
    /// character state would get modified.  For enemies, the AI would
    /// alter the state instead.
    /// </summary>
    protected abstract void Act();

    public virtual void FixedUpdate() {
        // Let player or AI modify character state first
        Act();

        // adjust velocity and fire events based on collisions (if any)
        float dt = Time.deltaTime;
        CollisionCheck(dt);

        // move the character
        float x = _transform.position.x + _character.velocity.x * dt;
        float y = _transform.position.y + _character.velocity.y * dt;
        _transform.position = new Vector3(x, y, 0);
        _character.position = new Vector2(x, y);
        _character.isWalking = _character.velocity.x != 0 || _character.velocity.y != 0;
    }

    /// <summary>
    /// Based on the _character's velocity, checks to see if the character would collide
    /// with something.  If so, then the character's velcoity and position are updated to
    /// prevent overlapping sprites.
    ///
    /// TODO: Fire collision events for _character and whatever _character collided with.
    /// </summary>
    /// <param name='deltaTime'>
    /// The amount of time that has passed since the last collision check.
    /// </param>
    protected virtual void CollisionCheck(float deltaTime) {

        Vector3 rayOrigin = this.collider.bounds.center;
        float absoluteDistance;
        RaycastHit hitInfo;


        if (_character.velocity.sqrMagnitude != 0) {
            Vector3 velocity = (Vector3)_character.velocity;
            Vector3 distance = velocity * deltaTime;
            absoluteDistance = distance.magnitude + _colliderBoundsOffsetX + _skinThickness;

            /*
            Debug.Log("Velocity: " + velocity);
            Debug.Log("Distance: " + distance);
            Debug.Log("AbsoluteDistance: " + absoluteDistance);
            */

            /*
            Debug.Log("Position: " + _transform.position);
            Debug.Log("Velocity: " + _character.velocity);
            Debug.Log("Velocity * Time: " + (_character.velocity * deltaTime));
            Debug.Log("Position + Velocity * Time: " + ((Vector2)_transform.position + _character.velocity * deltaTime));
            */

            Vector3 end = new Vector3(rayOrigin.x + velocity.x * absoluteDistance, rayOrigin.y + velocity.y * absoluteDistance);
            Debug.DrawLine(rayOrigin, end, Color.magenta);
            if (Physics.Raycast(rayOrigin, velocity, out hitInfo, absoluteDistance)) {

                Debug.Log("HIT!");
                Debug.Log("Distance: " + hitInfo.distance);
                Debug.Log("Normal: " + hitInfo.normal);

                _collisionHandler.OnCollision(hitInfo.collider, velocity, hitInfo.distance, hitInfo.normal);
            }
        }
    }

    protected void AddVelocity(Vector2 v) {
        _character.velocity.x += v.x;
        _character.velocity.y += v.y;
    }
}
