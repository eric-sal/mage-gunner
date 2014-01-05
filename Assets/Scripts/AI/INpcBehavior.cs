using UnityEngine;
using System.Collections;

public interface INpcBehavior {
    void doUpdate();

    void doFixedUpdate();

    INpcBehavior GetNextBehavior();
}
