using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.UI;

/** This code is from https://github.com/DANCH0U/Unity3D-Target-Lock-System/blob/main/TargetLock.cs 
 *  and has been modified to fit our project
 **/

public enum TargetDirection
{
    None,
    Left,
    Right
}

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
    private InputAction DpadLeftAction;
    private InputAction DpadRightAction;
    
    private int channeledAbility;
    
    private float maxAngle;
    [HideInInspector]
    private TargetDirection targetDirection = TargetDirection.None;
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
        DpadLeftAction = playerInput.actions["DpadLeft"];
        DpadRightAction = playerInput.actions["DpadRight"];


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
            if (isTargeting)
            {
                RemoveTarget();
            }
            else
            { 
                AssignTarget();
            }
        }
        // Change target with Dpad
        if (DpadLeftAction.triggered)
        {
            targetDirection = TargetDirection.Left;
            AssignTarget();
        }
        if (DpadRightAction.triggered)
        {
            targetDirection = TargetDirection.Right;
            AssignTarget();
        }
    }

    // Handles UN-locking from the current target
    private void RemoveTarget()
    {
        isTargeting = false;
        targetGroup.RemoveMember(currentTarget);
        targetGroup.RemoveMember(healthBar);
        currentTarget = null;
        targetDirection = TargetDirection.None;
    }

    // Handles locking on to a new target
    private void AssignTarget()
    {

        GameObject closest = ClosestTarget();
        if (closest)
        {
            // If changing target from an existing one, 
            // the camera stops locking on to the prev one
            if (currentTarget)
            {
                targetGroup.RemoveMember(currentTarget);
                targetGroup.RemoveMember(healthBar);
            }

            isTargeting = true;
            currentTarget = closest.transform;
            //Get's enemy model, which always has an animator attached
            CharacterController enemyCollision = currentTarget.GetComponent<CharacterController>();
            if (enemyCollision)
            {
                targetColliderCenter = enemyCollision.center * currentTarget.localScale.y;
            }
            else
            {
                /*If enemy does not have a BoxCollider, use this as aim adjustment instead.*/
                targetColliderCenter = new Vector3(0, 0.2f, 0);
            }
            // Vec = AddOffset(currentTarget.position);
            healthBar = currentTarget.transform.Find("UI Canvas").gameObject.transform;
            targetGroup.AddMember(currentTarget, 0.3f, 1f);
            targetGroup.AddMember(healthBar, 0.7f, 1f);
        }
    }

    private void LookAtTarget(Transform target) // sets new input value.
    {
        //If target is dead
        if (!currentTarget || currentTarget.tag != "Enemy")
        {
            RemoveTarget();
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
        }

        if(aimIcon)
            aimIcon.transform.position = mainCamera.WorldToScreenPoint(target.position + targetColliderCenter);

        // if too far or too close to enemy, deselect them
        if ((target.position - transform.position).magnitude < minDistance ||
            (target.position - transform.position).magnitude > maxDistance) { 
            RemoveTarget(); 
            return;
        }

        channeledAbility = player.ParseAbilityInput();
        /*Weird that this doesn't seem to work for abilities*/
        if (player.attackAction.triggered ||
            channeledAbility != -1)
        {
            // turn player towards enemy when they attack.
            /*Probably why turning towards the enemy feels slightly janky, but it works*/

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
            // Used if there are no existing targets and for cheking if the target is in view
            Vector3 playerDiff = go.transform.position - transform.position;
            // curDistance is the distance to the nearest enemy 
            float curDistance = 0f;
            
            if (currentTarget)
            {
                Vector3 targetDiff = go.transform.position - currentTarget.position;
                curDistance = targetDiff.magnitude;
            }
            else
                curDistance = playerDiff.magnitude;

            if (curDistance < distance)
            {
                Vector3 viewPos = mainCamera.WorldToViewportPoint(go.transform.position);
                Vector2 newPos = new Vector3(viewPos.x - 0.5f, viewPos.y - 0.5f);
                if (Vector3.Angle(playerDiff.normalized, mainCamera.transform.forward) < maxAngle)
                {
                    if (currentTarget)
                    {
                        // get closest target to the right or left of the current one
                        Vector3 cross = Vector3.Cross(mainCamera.transform.forward, go.transform.position - currentTarget.position);
                        if (targetDirection == TargetDirection.Right && cross.y > 0f) // TODO: or instead of else if 
                        {
                            closest = go;
                            distance = curDistance;
                        }
                        else if (targetDirection == TargetDirection.Left && cross.y < 0f)
                        {
                            closest = go;
                            distance = curDistance;
                        }
                    }
                    else
                    {
                        // Get closest target 
                        closest = go;
                        currAngle = Vector3.Angle(playerDiff.normalized, mainCamera.transform.forward.normalized);
                        distance = curDistance;
                    }
                }
            }
        }
        return closest;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}