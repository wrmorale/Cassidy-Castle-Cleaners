using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class BossPhase3Start : ActionNode
{
    protected override void OnStart() {
        MirrorBossMain mirrorBossRoot = context.mirrorBossScript;
        /*if(mirrorBossRoot.phase == 3){
            foreach (Transform child in mirrorBossRoot.transform){
                //add inactive mirrors into list and activate them
                MirrorBossMirror mirror = child.GetComponent<MirrorBossMirror>();
                if (mirror != null && !child.gameObject.activeSelf){
                    // Set the child GameObject to active
                    child.gameObject.SetActive(true);
                    mirrorBossRoot.mirrors.Add(mirror);
                    //this will trigger the action for setting the texture to whatever texture the other mirrors have
                    mirrorBossRoot.isHit(0,0); 
                }
            }
        }*/
        mirrorBossRoot.mirrors[0].mirrorAudioManager.playTeleportsfx();
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
