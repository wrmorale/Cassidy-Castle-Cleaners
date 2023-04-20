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
    private Vector3 currentPosition;
    private Quaternion initialRotation;
    private Quaternion rotationDirection;

    Vector3 eulerRotation;
    Quaternion newRotation;
    private float yRotation;
    private float rotationSpeed = 5.0f; //fall speed of the attack
    private float fallPercent;

    private float fallAnimationTime;
    private float riseAnimationTime;

    protected override void OnStart() {
        GameObject mirrorMain = GameObject.Find("BossRoot"); //gets mirror boss from scene
        if (mirrorMain == null) {
            Debug.LogError("Cannot find Boss");
        }

        posessedMirror = mirrorMain.GetComponent<MirrorBossMain>().currPosessedMirror; //gets current possesed mirror

        initialPosition = posessedMirror.transform.position; //gets initial position and rotation of posessed mirror
        initialRotation = posessedMirror.transform.rotation; 
        yRotation = posessedMirror.transform.rotation.eulerAngles.y;
        newRotation = posessedMirror.transform.rotation;

        fallAnimationTime = 5.0f; //temporary animation time for mirror falling (would just get this from animation attatched to the object)
        riseAnimationTime = 5.0f;
        animationIsPlaying = true;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        
        if(animationIsPlaying){
            if(fallAnimationTime > 0){
                //currently does damage based on the collision script attached to mirror prefab
                posessedMirror.basicAttackDamage = posessedMirror.abilities[0].abilityDamage; //uses the slam ability damage from ability prefab

                fallAnimationTime -= Time.deltaTime * rotationSpeed * 2; //pretty much the speed at which it falls
                fallPercent = (1 - fallAnimationTime / 5); //also tied to the way it falls
                rotationDirection = Quaternion.Euler(0, yRotation, -90 * fallPercent); //setting rotation direction
                currentPosition = initialPosition - new Vector3(0, fallPercent, 0); //moves the block down as it falls
                posessedMirror.transform.rotation = rotationDirection; //apply rotation and position to object
                posessedMirror.transform.position = currentPosition;

                if(fallAnimationTime <= 0) { //save the final rotation and position for the rising animation
                    newRotation = rotationDirection;
                    initialPosition = currentPosition;
                }

                return State.Running;
            }
            else if(riseAnimationTime > 0){ //for when it comes back up, looks a bit different but works in similar way
                posessedMirror.basicAttackDamage = 0;
                riseAnimationTime -= Time.deltaTime * rotationSpeed;
                fallPercent = (1 - riseAnimationTime / 5);
                rotationDirection = Quaternion.Euler(0, yRotation, initialRotation.z * fallPercent);
                currentPosition = initialPosition - new Vector3(0, -fallPercent, 0);
                posessedMirror.transform.rotation = Quaternion.Lerp(newRotation, rotationDirection, fallPercent);
                posessedMirror.transform.position = currentPosition;

                if(riseAnimationTime <= 0) {
                    newRotation = rotationDirection;
                    initialPosition = currentPosition;
                }

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
