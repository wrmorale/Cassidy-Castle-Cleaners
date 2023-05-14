using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class BossPhase3Start : ActionNode
{
    protected override void OnStart() {
        MirrorBossMain mirrorBossRoot = context.mirrorBossScript;
        if(mirrorBossRoot.phase == 3){
            foreach (Transform child in mirrorBossRoot.transform){
                // Set the child GameObject to active
                child.gameObject.SetActive(true);
                MirrorBossMirror mirror = child.GetComponent<MirrorBossMirror>();
                if (mirror != null){
                    mirrorBossRoot.mirrors.Add(mirror);
                }
            }
        }
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
