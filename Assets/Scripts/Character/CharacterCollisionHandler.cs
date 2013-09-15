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

        //float theta = Vector3.Angle(fromDirection, normal);
        //Debug.Log(theta);
        //float radians = theta * Mathf.Deg2Rad;
        //Vector3 adjacentVector = new Vector3(normal.x * Mathf.Abs(Mathf.Cos(radians)), normal.y * Mathf.Abs(Mathf.Sin(radians)));


        Vector3 adjacentVector = normal;
        Debug.Log("From: " + fromDirection.normalized);
        Debug.Log("Adjacent: " + adjacentVector);
        Vector3 oppositeVector = fromDirection.normalized + adjacentVector;
        Debug.Log("Opposite: " + oppositeVector);

        //float delta = Vector3.Angle(fromDirection, oppositeVector);
        //Debug.Log(delta);

        _character.velocity.x = oppositeVector.x;
        _character.velocity.y = oppositeVector.y;

        //Debug.Log(_character.velocity.x);
        //Debug.Log(_character.velocity.y);

        //Vector3 distanceOffset = fromDirection.normalized * distance * -1;
        //_transform.position = new Vector3(_transform.position.x + distanceOffset.x, _transform.position.y + distanceOffset.y);
    }
}
