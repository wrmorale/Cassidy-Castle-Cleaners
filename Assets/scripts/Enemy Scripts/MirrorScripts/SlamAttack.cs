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

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float elapsedTime = 0.0f;

    protected override void OnStart() {
        GameObject mirrorMain = GameObject.Find("BossRoot");
        if (mirrorMain == null) {
            Debug.LogError("Cannot find Boss");
        }
        posessedMirror = mirrorMain.GetComponent<MirrorBossMain>().currPosessedMirror;
        initialRotation = posessedMirror.transform.rotation;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        state = State.Running;
        elapsedTime += Time.deltaTime;
        float step = fallSpeed * Time.deltaTime;

        /* Move the object downward
        Vector3 newPosition = posessedMirror.transform.position;
        newPosition.y -= step;
        posessedMirror.transform.position = newPosition;*/

        // Rotate the object forward gradually
        Quaternion targetRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        Quaternion newRotation = Quaternion.RotateTowards(posessedMirror.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        posessedMirror.transform.rotation = newRotation;
        
        return State.Success;
    }
}
