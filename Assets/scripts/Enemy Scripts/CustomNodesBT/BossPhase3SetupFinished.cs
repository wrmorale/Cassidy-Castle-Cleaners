using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class BossPhase3SetupFinished : ActionNode
{
    /*Called the first time this node is ran on a tick.
    If a node returns RUNNING it will not call OnStart()
    on the next tick*/
    protected override void OnStart() {
    }

    /*Called after OnUpdate if the node returns SUCCESS or 
    FAILURE. Also called if the current behavior using the
    node function Abort()*/
    protected override void OnStop() {
    }

    /*Called every tick that this node is executed*/
    protected override State OnUpdate() {
        context.mirrorBossScript.setupPhase3 = true;
        context.mirrorBossScript.abortBT();
        return State.Success;
    }
}
