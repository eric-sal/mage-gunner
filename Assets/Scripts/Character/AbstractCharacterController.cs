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

    const float RAY_GAP = 0.2f;
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

        if (_character.velocity.sqrMagnitude != 0) {
            Vector3 velocity = (Vector3)_character.velocity;
            Vector3 distance = velocity * deltaTime;
            float rayLength = distance.magnitude + _skinThickness;
            RaycastHit hitInfo;

            Bounds b = this.collider.bounds;
            Vector3 center = b.center;
            float ex = b.extents.x;
            float ey = b.extents.y;

            var topLeft = new Vector3(center.x - ex + _skinThickness, center.y + ey - _skinThickness);
            var topRight = new Vector3(center.x + ex - _skinThickness, center.y + ey - _skinThickness);
            var bottomLeft = new Vector3(center.x - ex + _skinThickness, center.y - ey + _skinThickness);
            var bottomRight = new Vector3(center.x + ex - _skinThickness, center.y - ey + _skinThickness);

            if (velocity.x > 0) {
                // Moving right
                if (VerticalSweep(bottomRight, topRight, velocity, out hitInfo, rayLength)) {
                    _collisionHandler.OnCollision(hitInfo.collider, velocity, hitInfo.distance, hitInfo.normal);
                    CollisionCheck(deltaTime);
                }
                
            } else if (velocity.x < 0) {
                // Moving left
                if (VerticalSweep(bottomLeft, topLeft, velocity, out hitInfo, rayLength)) {
                    _collisionHandler.OnCollision(hitInfo.collider, velocity, hitInfo.distance, hitInfo.normal);
                    CollisionCheck(deltaTime);
                }
            }

            if (velocity.y > 0) {
                // Moving up
                if (HorizontalSweep(topLeft, topRight, velocity, out hitInfo, rayLength)) {
                    _collisionHandler.OnCollision(hitInfo.collider, velocity, hitInfo.distance, hitInfo.normal);
                    CollisionCheck(deltaTime);
                }
            } else if (velocity.y < 0) {
                // Moving down
                if (HorizontalSweep(bottomLeft, bottomRight, velocity, out hitInfo, rayLength)) {
                    _collisionHandler.OnCollision(hitInfo.collider, velocity, hitInfo.distance, hitInfo.normal);
                    CollisionCheck(deltaTime);
                }
            }
        }
    }

    protected void AddVelocity(Vector2 v) {
        _character.velocity.x += v.x;
        _character.velocity.y += v.y;
    }

    private bool HorizontalSweep(Vector3 startPoint, Vector3 endPoint, Vector3 direction, out RaycastHit hitInfo, float rayLength) {

        float x = startPoint.x;
        float y = startPoint.y;

        RaycastHit lastHit = new RaycastHit();
        float lastDistance = float.MaxValue;

        for (x = startPoint.x; x <= endPoint.x; x += RAY_GAP) {
            var start = new Vector3(x, y);
            var end = new Vector3(start.x + direction.x * rayLength, start.y + direction.y * rayLength);

            Debug.DrawLine(start, end, Color.red, 0.25f);
            if (Physics.Raycast(start, direction, out hitInfo, rayLength) && hitInfo.distance < lastDistance) {
                lastDistance = hitInfo.distance;
                lastHit = hitInfo;
            }
        }
     
        if (x != endPoint.x) {
            var start = new Vector3(endPoint.x, y);
            var end = new Vector3(start.x + direction.x * rayLength, start.y + direction.y * rayLength);
            Debug.DrawLine(start, end, Color.blue, 0.25f);
     
            if (Physics.Raycast(endPoint, direction, out hitInfo, rayLength) && hitInfo.distance < lastDistance) {
                lastDistance = hitInfo.distance;
                lastHit = hitInfo;
            }
        }
     
        if (lastDistance != float.MaxValue) {
            hitInfo = lastHit;
            return true;
        }
     
        hitInfo = new RaycastHit();
        return false;
    }

    private bool VerticalSweep(Vector3 startPoint, Vector3 endPoint, Vector3 direction, out RaycastHit hitInfo, float rayLength) {
        
        float x = startPoint.x;
        float y = startPoint.y;
     
        RaycastHit lastHit = new RaycastHit();
        float lastDistance = float.MaxValue;

        for (y = startPoint.y; y <= endPoint.y; y += RAY_GAP) {
            var start = new Vector3(x, y);
            var end = new Vector3(start.x + direction.x * rayLength, start.y + direction.y * rayLength);

            Debug.DrawLine(start, end, Color.cyan, 0.25f);
            if (Physics.Raycast(start, direction, out hitInfo, rayLength) && hitInfo.distance < lastDistance) {
                lastDistance = hitInfo.distance;
                lastHit = hitInfo;
            }
        }
     
        if (y != endPoint.y) {
            var start = new Vector3(x, endPoint.y);
            var end = new Vector3(start.x + direction.x * rayLength, start.y + direction.y * rayLength);
            Debug.DrawLine(start, end, Color.blue, 0.25f);   
         
            if (Physics.Raycast(endPoint, direction, out hitInfo, rayLength) && hitInfo.distance < lastDistance) {
                lastDistance = hitInfo.distance;
                lastHit = hitInfo;
            }
        }
     
        if (lastDistance != float.MaxValue) {
            hitInfo = lastHit;
            return true;
        }
     
        hitInfo = new RaycastHit();
        return false;
    }
}
