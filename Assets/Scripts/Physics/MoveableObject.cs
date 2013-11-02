using UnityEngine;
using System.Collections;

/// <summary>
/// Attach this script to any GameObject capable of moving such as characters,
/// moving platforms, or projectiles.
/// </summary>
public class MoveableObject : MonoBehaviour {

    const float RAY_GAP = 0.2f;
    const float SKIN_THICKNESS = 0.02f;

    public Vector3 velocity;

    protected BaseCharacterController _characterController;
    protected BaseCollisionHandler _collisionHandler;

    void Awake() {
        _characterController = GetComponent<BaseCharacterController>();
        _collisionHandler = GetComponent<BaseCollisionHandler>();
    }

    public bool isMovingRight {
        get { return this.velocity.x > 0; }
    }

    public bool isMovingLeft {
        get { return this.velocity.x < 0; }
    }

    public bool isMovingUp {
        get { return this.velocity.y > 0; }
    }

    public bool isMovingDown {
        get { return this.velocity.y < 0; }
    }

    public Vector3 position {
        get { return this.transform.position; }
    }

    public virtual void FixedUpdate() {
        if (_characterController == null) {
            // this is an "uncontrolled" moveable object
            float dt = Time.deltaTime;
            Move(velocity, dt);
        }
    }

    /// <summary>
    /// Move this object at the given velocity for the given amount of time
    /// or until a collision with another object occurs.  If a collision does
    /// occur, inform the collision handler.
    /// </summary>
    public virtual void Move(Vector3 velocity, float deltaTime) {

        if (velocity.x == 0f && velocity.y == 0f) {
            return;
        }

        Vector3 distance = velocity * deltaTime;
        float rayLength = distance.magnitude + SKIN_THICKNESS;

        Bounds colliderBounds = this.collider.bounds;
        Vector3 colliderCenter = colliderBounds.center;
        float colliderExtentsX = colliderBounds.extents.x;
        float ColliderExtentsY = colliderBounds.extents.y;

        var topLeft = new Vector3(
            colliderCenter.x - colliderExtentsX + SKIN_THICKNESS,
            colliderCenter.y + ColliderExtentsY - SKIN_THICKNESS
        );
        var topRight = new Vector3(
            colliderCenter.x + colliderExtentsX - SKIN_THICKNESS,
            colliderCenter.y + ColliderExtentsY - SKIN_THICKNESS
        );
        var bottomLeft = new Vector3(
            colliderCenter.x - colliderExtentsX + SKIN_THICKNESS,
            colliderCenter.y - ColliderExtentsY + SKIN_THICKNESS
        );
        var bottomRight = new Vector3(
            colliderCenter.x + colliderExtentsX - SKIN_THICKNESS,
            colliderCenter.y - ColliderExtentsY + SKIN_THICKNESS
        );

        RaycastHit hHitInfo = new RaycastHit();
        bool hadHorzCollision = false;
        if (this.isMovingRight) {
            hadHorzCollision = VerticalSweep(bottomRight, topRight, velocity, out hHitInfo, rayLength);
        } else if (this.isMovingLeft) {
            hadHorzCollision = VerticalSweep(bottomLeft, topLeft, velocity, out hHitInfo, rayLength);
        }

        RaycastHit vHitInfo = new RaycastHit();
        bool hadVertCollision = false;
        if (this.isMovingUp) {
            hadVertCollision = HorizontalSweep(topLeft, topRight, velocity, out vHitInfo, rayLength);
        } else if (this.isMovingDown) {
            hadVertCollision = HorizontalSweep(bottomLeft, bottomRight, velocity, out vHitInfo, rayLength);
        }

        if (!hadHorzCollision && !hadVertCollision) {
            // no collisions, move the object the desired distance
            Vector3 p = this.transform.position;
            this.transform.position = new Vector3(p.x + distance.x, p.y + distance.y);
        } else {
            // collision, use the shortest distance
            RaycastHit hitInfo = vHitInfo;
            if (hadVertCollision && hadHorzCollision) {
                if (hHitInfo.distance < vHitInfo.distance) {
                    hitInfo = hHitInfo;
                }
            } else if (hadHorzCollision) {
                hitInfo = hHitInfo;
            }

            float actualDistance = hitInfo.distance - SKIN_THICKNESS;
            if (actualDistance > 0f) {
                // we cannot move the full distance we originally wanted to
                _collisionHandler.OnCollision(hitInfo.collider, velocity, actualDistance, hitInfo.normal, deltaTime);
            } else {
                // cannot move, collided as soon we tried to move
                _collisionHandler.OnCollision(hitInfo.collider, velocity, 0f, hitInfo.normal, deltaTime);
            }
        }

    }

    protected void AddVelocity(Vector2 v) {
        this.velocity = new Vector3(this.velocity.x + v.x, this.velocity.y + v.y);
    }

    /// <summary>
    /// Casts rays with given length and in given direction at predefined intervals from startPoint to endPoint.
    /// startPoint and endPoint should have different y values
    /// </summary>
    /// <returns>
    /// True if any ray hits a collider
    /// </returns>
    private bool VerticalSweep(Vector3 startPoint, Vector3 endPoint, Vector3 rayDirection, out RaycastHit hitInfo, float rayLength) {

        float x = startPoint.x;
        float y = startPoint.y;
        float rayLengthX = rayDirection.x * rayLength;
        float rayLengthY = rayDirection.y * rayLength;

        RaycastHit lastHit = new RaycastHit();
        float lastDistance = float.MaxValue;

        for (y = startPoint.y; y <= endPoint.y; y += RAY_GAP) {
            var start = new Vector3(x, y);
            var end = new Vector3(start.x + rayLengthX, start.y + rayLengthY);

            Debug.DrawLine(start, end, Color.cyan, 0.25f);
            if (Physics.Raycast(start, rayDirection, out hitInfo, rayLength) && hitInfo.distance < lastDistance) {
                lastDistance = hitInfo.distance;
                lastHit = hitInfo;
            }
        }

        if (y != endPoint.y) {
            // we didn't cast a ray originating from the given 'endPoint', do so now
            var start = new Vector3(x, endPoint.y);
            var end = new Vector3(start.x + rayLengthX, start.y + rayLengthY);
            Debug.DrawLine(start, end, Color.blue, 0.25f);

            if (Physics.Raycast(endPoint, rayDirection, out hitInfo, rayLength) && hitInfo.distance < lastDistance) {
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

    /// <summary>
    /// Casts rays with given length and in given direction at predefined intervals from startPoint to endPoint.
    /// startPoint and endPoint should have different x values
    /// </summary>
    /// <returns>
    /// True if any ray hits a collider
    /// </returns>
    private bool HorizontalSweep(Vector3 startPoint, Vector3 endPoint, Vector3 rayDirection, out RaycastHit hitInfo, float rayLength) {

        float x = startPoint.x;
        float y = startPoint.y;
        float rayLengthX = rayDirection.x * rayLength;
        float rayLengthY = rayDirection.y * rayLength;

        RaycastHit lastHit = new RaycastHit();
        float lastDistance = float.MaxValue;

        for (x = startPoint.x; x <= endPoint.x; x += RAY_GAP) {
            var start = new Vector3(x, y);
            var end = new Vector3(start.x + rayLengthX, start.y + rayLengthY);

            Debug.DrawLine(start, end, Color.red, 0.25f);
            if (Physics.Raycast(start, rayDirection, out hitInfo, rayLength) && hitInfo.distance < lastDistance) {
                lastDistance = hitInfo.distance;
                lastHit = hitInfo;
            }
        }

        if (x != endPoint.x) {
            // we didn't cast a ray originating from the given 'endPoint', do so now
            var start = new Vector3(endPoint.x, y);
            var end = new Vector3(start.x + rayLengthX, start.y + rayLengthY);
            Debug.DrawLine(start, end, Color.blue, 0.25f);

            if (Physics.Raycast(endPoint, rayDirection, out hitInfo, rayLength) && hitInfo.distance < lastDistance) {
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

