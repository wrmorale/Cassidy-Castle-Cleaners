using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class SlamAttack : ActionNode
{
    public MirrorBossMirror posessedMirror;
    public float fallSpeed = 1.0f;
    public float rotationSpeed = 5.0f;
    public float animationTime;
    public bool animationIsPlaying = false;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float fallForwardRotation;

    protected override void OnStart() {
        GameObject mirrorMain = GameObject.Find("BossRoot");
        if (mirrorMain == null) {
            Debug.LogError("Cannot find Boss");
        }
        animationTime = 5;
        animationIsPlaying = true;
        posessedMirror = mirrorMain.GetComponent<MirrorBossMain>().currPosessedMirror;
        initialRotation = posessedMirror.transform.rotation;
        fallForwardRotation = initialRotation.eulerAngles.z;;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if(animationIsPlaying){
            if(animationTime > 0){
                animationTime -= Time.deltaTime;
                fallForwardRotation -= 0.1f;
                posessedMirror.transform.rotation = Quaternion.Euler(fallForwardRotation, 0.0f, 0.0f);
                return State.Running;
            }
            else{
                animationTime = 0;
                animationIsPlaying = false;
                return State.Success;
            }
        }

        return State.Running;        
    }
}
