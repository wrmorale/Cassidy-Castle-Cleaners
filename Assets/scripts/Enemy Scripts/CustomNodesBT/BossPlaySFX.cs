using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class BossPlaySFX : ActionNode
{
    protected override void OnStart() {
        MirrorBossMain mirrorBoss = context.mirrorBossScript;
        foreach(MirrorBossMirror mirror in mirrorBoss.mirrors){ //can be changed to only the main one but does all mirrors for now
            mirror.mirrorAudioManager.playLaughsfx();
        }
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
