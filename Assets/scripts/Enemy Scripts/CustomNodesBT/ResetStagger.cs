using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class ResetStagger : ActionNode
{
    protected override void OnStart() {
        context.enemy.isStaggered = false;
        context.enemy.currentStaggerAmount = 0;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
