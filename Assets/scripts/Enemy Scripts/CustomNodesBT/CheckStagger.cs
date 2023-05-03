using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class CheckStagger : DecoratorNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
        context.enemy.isStaggered = false;
    }

    protected override State OnUpdate() {
        if (context.enemy.isStaggered == true)
            return child.Update();
        else
            return State.Failure;
    }
}
