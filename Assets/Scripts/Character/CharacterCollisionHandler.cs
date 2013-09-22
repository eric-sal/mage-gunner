using UnityEngine;
using System.Collections;

public class CharacterCollisionHandler : AbstractCollisionHandler {
    protected CharacterState _character;
    protected Transform _transform;
    protected float _colliderBoundsOffsetX;
    protected float _colliderBoundsOffsetY;

    public override void Awake() {
        base.Awake();
        _character = GetComponent<CharacterState>();
        _transform = this.transform;
        _colliderBoundsOffsetX = this.collider.bounds.extents.x;
        _colliderBoundsOffsetY = this.collider.bounds.extents.y;
    }

    /// <summary>
    /// Stop the character's movement in the direction the collision came from.
    /// </summary>
    public override void HandleCollision(Collider collidedWith, Vector3 fromDirection, float distance, Vector3 normal) {
        float hDistance = 0;
        Vector3 hDirection = Vector3.Normalize(new Vector3(fromDirection.x, 0));
        float vDistance = 0;
        Vector3 vDirection = Vector3.Normalize(new Vector3(0, fromDirection.y));

        float theta = Vector3.Angle(fromDirection, normal);

        // If the wall or floor we've just run into is close enough to exactly opposite
        // to our velocity vector, then we'll just just stop our forward progress.
        if (theta <= 160f) {
            // QUESTION: Does it make sense to pass the fromDirection.magnitude to the collision
            // handler methods since calculating the magnitude is an expensive operation?
            float adjacentVectorMagnitude = fromDirection.magnitude * Mathf.Abs(Mathf.Cos(theta * Mathf.Deg2Rad));
            Vector3 adjacentVector = normal * adjacentVectorMagnitude;
            Vector3 oppositeVector = fromDirection + adjacentVector;

            _character.velocity.x = oppositeVector.x;
            _character.velocity.y = oppositeVector.y;

            float hAngle = Vector3.Angle(hDirection, fromDirection);
            float vAngle = Vector3.Angle(vDirection, fromDirection);

            hDistance = (distance - 0.01f) * Mathf.Cos(hAngle * Mathf.Deg2Rad) * hDirection.x;
            vDistance = (distance - 0.01f) * Mathf.Cos(vAngle * Mathf.Deg2Rad) * vDirection.y;
        } else {
            hDistance = (distance - 0.01f) * hDirection.x;
            vDistance = (distance - 0.01f) * vDirection.y;

            _character.velocity.x = 0;
            _character.velocity.y = 0;
        }

        _transform.position = new Vector3(_transform.position.x + hDistance, _transform.position.y + vDistance);
    }
}
