using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class MirrorEnemySpawn : ActionNode
{
    protected override void OnStart() {
        MirrorBossMain mirrorBossRoot = context.mirrorBossScript;
        mirrorBossRoot.spawnEnemies();
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
