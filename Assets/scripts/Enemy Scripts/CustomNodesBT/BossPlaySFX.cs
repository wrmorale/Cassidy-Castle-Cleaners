using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class BossPlaySFX : ActionNode
{
    protected override void OnStart() {
        MirrorBossMain mirrorBoss = context.mirrorBossScript;
        mirrorBoss.currPosessedMirror.mirrorAudioManager.playLaughsfx(); //Should only play laugh once so that phase 2 isn't louder
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
