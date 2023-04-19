using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class SlamAttack : ActionNode
{
    private MirrorBossMirror posessedMirror;

    private float animationTime;
    private bool animationIsPlaying = false;
    
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Quaternion rotationDirection;

    Vector3 eulerRotation;
    Quaternion newRotation;

    private float yRotation;
    private float rotationSpeed = 3.5f;

    private float fallAnimationTime;
    private float riseAnimationTime;

    protected override void OnStart() {
        GameObject mirrorMain = GameObject.Find("BossRoot"); //gets mirror boss from scene
        if (mirrorMain == null) {
            Debug.LogError("Cannot find Boss");
        }

        posessedMirror = mirrorMain.GetComponent<MirrorBossMain>().currPosessedMirror; //gets current possesed mirror

        initialPosition = posessedMirror.transform.position;
        initialRotation = posessedMirror.transform.rotation; //gets initial rotation of posessed mirror
        yRotation = posessedMirror.transform.rotation.eulerAngles.y;
        newRotation = posessedMirror.transform.rotation;

        fallAnimationTime = 5.0f; //temp animation time for mirror falling
        riseAnimationTime = 5.0f;
        animationIsPlaying = true;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if(animationIsPlaying){
            if(fallAnimationTime > 0){
                fallAnimationTime -= Time.deltaTime * rotationSpeed * 2; //faster fall speed lead to faster animation
                rotationDirection = Quaternion.Euler(0, yRotation, -90);
                posessedMirror.transform.rotation = Quaternion.Lerp(initialRotation, rotationDirection, (1 - fallAnimationTime / 5)); //kinda got chat gpt to help me with this line
                //work in progress for getting the position of the block to be positioned better as it falls
                //posessedMirror.transform.position = Vector3.Lerp(initialPosition, posessedMirror.transform.position + posessedMirror.transform.forward * 5, (1 - animationTime / 5));
                newRotation = posessedMirror.transform.rotation;
                return State.Running;
            }
            else if(riseAnimationTime > 0){
                riseAnimationTime -= Time.deltaTime * rotationSpeed;
                rotationDirection = Quaternion.Euler(0, yRotation, 0);
                posessedMirror.transform.rotation = Quaternion.Lerp(newRotation, rotationDirection, (1 - riseAnimationTime / 5));
                return State.Running;
            }
            else{ //returns success when animation is finished.
                fallAnimationTime = 0;
                riseAnimationTime = 0;
                animationIsPlaying = false;
                return State.Success;
            }
        }

        return State.Running;        
    }
}
