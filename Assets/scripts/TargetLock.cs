using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.UI;

/** This code is from https://github.com/DANCH0U/Unity3D-Target-Lock-System/blob/main/TargetLock.cs 
 *  and has been modified to fit our project
 **/

public class TargetLock : MonoBehaviour
{ 
    [Header("Objects")]
    [Space]
    [SerializeField] private Camera mainCamera;            // your main camera object.
    [SerializeField] private CinemachineFreeLook cinemachineFreeLook; //cinemachine free lock camera object.
    [SerializeField] private CinemachineTargetGroup targetGroup;
    [Space]
    [Header("UI")]
    [SerializeField] private Image aimIcon;  // ui image of aim icon u can leave it null.
    [Space]
    [Header("Settings")]
    [Space]
    [SerializeField] private string enemyTag; // the enemies tag.
    [SerializeField] private Vector2 targetLockOffset;
    [SerializeField] private float minDistance; // minimum distance to stop rotation if you get close to target
    [SerializeField] private float maxDistance;
    
    public bool isTargeting;
    private playerController player;
    private CharacterController controller;
    private PlayerInput playerInput;
    private InputAction lockonAction;
    private InputAction cameraAction;
    private int channeledAbility;
    
    private float maxAngle;
    [HideInInspector]
    public Transform currentTarget;
    public Transform healthBar;
    public Vector3 targetColliderCenter;
    private float mouseX;
    private float mouseY;

    void Start()
    {
        maxAngle = 90f; // always 90 to target enemies in front of camera.

        player = gameObject.GetComponent<playerController>();
        controller  = gameObject.GetComponent<CharacterController>();
        playerInput = gameObject.GetComponent<PlayerInput>();

        lockonAction = playerInput.actions["LockOn"];
        cameraAction = playerInput.actions["Camera"];

        isTargeting = false;
    }

    void Update()
    {
        if (!isTargeting)
        {
            Vector2 input = cameraAction.ReadValue<Vector2>();
            mouseX = input.x;
            mouseY = input.y;
        }
        else
        {
            // handles being locked on a target
            LookAtTarget(currentTarget); 
        }

        if (aimIcon) 
            aimIcon.gameObject.SetActive(isTargeting);

        if (lockonAction.triggered)
        {
            Debug.Log("targeting");
            AssignTarget();
        }
    }

    private void AssignTarget()
    {
        if (isTargeting)
        {
            isTargeting = false;
            targetGroup.RemoveMember(currentTarget);
            targetGroup.RemoveMember(healthBar);
            currentTarget = null;
            return;
        }

        GameObject closest = ClosestTarget();
        if (closest)
        {
            isTargeting = true;
            currentTarget = closest.transform;
            //Get's enemy model, which always has an animator attached
            BoxCollider enemyCollision = currentTarget.GetComponent<BoxCollider>();
            if (enemyCollision)
            {
                targetColliderCenter = enemyCollision.center * currentTarget.localScale.y;
            }
            else
            {
                /*If enemy does not have a BoxCollider, use this as aim adjustment instead.*/
                targetColliderCenter = new Vector3(0, 0.2f, 0);
                Debug.LogWarning(currentTarget + " does not have a BoxCollider to use for lock-on targeting");
            }
            // Vec = AddOffset(currentTarget.position);
            healthBar = currentTarget.transform.Find("UI Canvas").gameObject.transform;
            targetGroup.AddMember(currentTarget, 0.3f, 1f);
            targetGroup.AddMember(healthBar, 0.7f, 1f);
            Debug.Log("current target: "+ currentTarget);
        }
    }

    private void LookAtTarget(Transform target) // sets new input value.
    {
        if (!currentTarget)
        {
            isTargeting = false;
            return;
        }

        BoxCollider enemyCollision = currentTarget.GetComponent<BoxCollider>();
        if (enemyCollision)
        {
            targetColliderCenter = enemyCollision.center * currentTarget.localScale.y;
        }
        else
        {
            /*If enemy does not have a BoxCollider, use this as aim adjustment instead.*/
            targetColliderCenter = new Vector3(0, 0.2f, 0);
            Debug.LogWarning(currentTarget + " does not have a BoxCollider to use for lock-on targeting");
        }

        if(aimIcon)
            aimIcon.transform.position = mainCamera.WorldToScreenPoint(target.position + targetColliderCenter);

        // if too far or too close to enemy, deselect them
        if ((target.position - transform.position).magnitude < minDistance ||
            (target.position - transform.position).magnitude > maxDistance) { 
            AssignTarget(); 
            return;
        }

        //Debug.Log(player.ParseAbilityInput());
        channeledAbility = player.ParseAbilityInput();
        /*Weird that this doesn't seem to work for abilities*/
        if (player.attackAction.triggered ||
            channeledAbility != -1)
        {
            // turn player towards enemy when they attack.
            /*Probably why turning towards the enemy feels slightly janky, but it works*/
            //Debug.Log("Redirected attack towards target");

            Quaternion newRotation = Quaternion.LookRotation(player.toTargetPosition(), controller.transform.up);
            newRotation = Quaternion.Euler(0, newRotation.eulerAngles.y, 0);
            controller.transform.rotation = newRotation;
            //controller.transform.LookAt(currentTarget); //Old rotate method if we want it
        }
    }


    private GameObject ClosestTarget() // this is modified func from unity Docs ( Gets Closest Object with Tag ). 
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject closest = null;
        float distance = maxDistance;
        float currAngle = maxAngle;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.magnitude;
            if (curDistance < distance)
            {
                //Debug.Log("In range.");
                Vector3 viewPos = mainCamera.WorldToViewportPoint(go.transform.position);
                Vector2 newPos = new Vector3(viewPos.x - 0.5f, viewPos.y - 0.5f);
                //Debug.Log(Vector3.Angle(diff.normalized, mainCamera.transform.forward) < maxAngle);
                if (Vector3.Angle(diff.normalized, mainCamera.transform.forward) < maxAngle)
                {
                    //Debug.Log("in View");
                    closest = go;
                    currAngle = Vector3.Angle(diff.normalized, mainCamera.transform.forward.normalized);
                    distance = curDistance;
                }
            }
        }
        Debug.Log(closest);
        return closest;
    }

    private Vector3 AddOffset(Vector3 target)
    {
        Vector3 offset = targetLockOffset;
        Debug.Log(offset);
        offset = target + offset;
        return offset;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}