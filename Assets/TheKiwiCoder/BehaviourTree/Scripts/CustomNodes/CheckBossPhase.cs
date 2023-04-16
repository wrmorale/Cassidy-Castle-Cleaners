using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class CheckBossPhase : DecoratorNode
{
    public int phase = 0;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (context.mirrorBossScript.phase == phase)
            return child.Update();
        else
            return State.Failure;
    }
}
