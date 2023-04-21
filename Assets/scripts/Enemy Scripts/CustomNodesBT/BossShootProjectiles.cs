using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class BossShootProjectiles : ActionNode
{
    //Make ALL mirrors fire projectiles for X seconds at a rate of X projectiles per second

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        MirrorBossMain mirrorBossRoot = context.mirrorBossScript;
        return State.Success;
    }
}
