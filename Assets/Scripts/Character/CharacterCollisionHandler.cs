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
        float vDistance = 0;
     
        // TODO: Take into account input in both x and y directions... AT THE SAME TIME!
     
        // a collision in the direction we are moving means we should stop moving
        if (fromDirection == Vector3.right || fromDirection == Vector3.left) {
            hDistance = (distance - _colliderBoundsOffsetX) * fromDirection.x;
            _transform.position = new Vector3(_transform.position.x + hDistance, _transform.position.y, 0);
         
            // Determine the angle of the sloped surface from the normal.
            float angle = Vector3.Angle(fromDirection, normal) - 90;
            if (angle > 70) {
                // If the surface we collied with is a (near) vertical wall,
                // then stop our forward progress.
                _character.velocity.x = 0;
            } else {
                // Otherwise, this is a sloped surface.
                float radians = angle * Mathf.Deg2Rad;
                _character.velocity.x = Mathf.Abs(Mathf.Cos(radians) * _character.velocity.x) * fromDirection.x;
                _character.velocity.y = Mathf.Abs(Mathf.Sin(radians) * _character.velocity.x) * fromDirection.x;
            }
        } else if (fromDirection == Vector3.up || fromDirection == Vector3.down) {

            vDistance = (distance - _colliderBoundsOffsetY) * fromDirection.y;
         
            if (normal.x == 0) {
                _character.velocity.y = 0;
            }
        }
    }
}
