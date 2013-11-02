using UnityEngine;
using System.Collections;

public class CharacterCollisionHandler : AbstractCollisionHandler {

    protected MoveableObject _moveable;

    public override void Awake() {
        base.Awake();
        _moveable = GetComponent<MoveableObject>();
    }

    /// <summary>
    /// Stop the character's movement in the direction the collision came from.
    /// </summary>
    public override void HandleCollision(Collider collidedWith, Vector3 impactVelocity, float distance, Vector3 normal, float deltaTime) {

        // only move the distance until we hit another collider
        float speed = impactVelocity.magnitude;
        float timeSpentMoving = distance / speed;
        Vector3 tempDistance = impactVelocity * timeSpentMoving;
        Vector3 p = this.transform.position;
        this.transform.position = new Vector3(p.x + tempDistance.x, p.y + tempDistance.y);

        float remainingTime = deltaTime - timeSpentMoving;
        if (remainingTime <= 0f) {
            // we are out of time
            return;
        }

        float theta = Vector3.Angle(impactVelocity, normal);

        // If the wall or floor we've just run into is close enough to exactly opposite
        // to our velocity vector, then we'll just just stop our forward progress.
        if (theta <= 160f) {
            float radians = theta * Mathf.Deg2Rad;
            float adjacentVectorMagnitude = speed * Mathf.Abs(Mathf.Cos(radians));
            Vector3 adjacentVector = normal * adjacentVectorMagnitude;
            Vector3 oppositeVector = impactVelocity + adjacentVector;

            _moveable.Move(oppositeVector, remainingTime);
        }
    }
}