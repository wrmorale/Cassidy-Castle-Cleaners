using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class BossSwapMirror : ActionNode
{
    public int mirrorIndexToPosess = 0; //If -1, will posess no mirror
    public bool posessRandomMirror = true;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (posessRandomMirror)
            context.mirrorBossScript.PosessMirrorRandom();
        else if (mirrorIndexToPosess < 0)
            context.mirrorBossScript.StopPosessingMirrors();
        else
            context.mirrorBossScript.PosessMirror(mirrorIndexToPosess);
        return State.Success;
    }
}
