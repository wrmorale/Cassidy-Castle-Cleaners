using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class CheckBossPhase : DecoratorNode
{
    public int[] phases = new int[1];

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        foreach(int phase in phases)
        {
            if(context.mirrorBossScript.phase == phase)
            {
                return child.Update();
            }
        }
        return State.Failure;
    }
}
