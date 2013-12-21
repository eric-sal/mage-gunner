using UnityEngine;
using System.Collections;

public static class VectorExtensions {
    // * Vector3 Extensions *
    public static Vector2 Vector2D(this Vector3 v) {
        return new Vector2(v.x, v.y);
    }

    // * Vector2 Extensions *
    public static Vector3 Vector3D(this Vector2 v) {
        return new Vector3(v.x, v.y);
    }
}
