﻿using UnityEngine;
using System.Collections;

public class IdleBehavior : BaseBehavior {
    protected override void _FixedUpdate() {
        _controller.character.velocity = Vector3.zero;
        _rigidbody.velocity = Vector3.zero;
    }
}
