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
    [SerializeField] private LayerMask transparentObjects; // objects that should not block raycast.
    [SerializeField] private float cameraHeight = 0.5f; // height the camera moves to when locking on
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
        controller = gameObject.GetComponent<CharacterController>();
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

            // Vec = AddOffset(currentTarget.position);
            healthBar = currentTarget.transform.Find("UI Canvas").gameObject.transform;
            targetGroup.AddMember(currentTarget, 0.3f, 1f);
            targetGroup.AddMember(healthBar, 0.7f, 1f);

            cinemachineFreeLook.m_YAxis.Value = cameraHeight;
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

        if (aimIcon)
            aimIcon.transform.position = mainCamera.WorldToScreenPoint(target.position + targetColliderCenter);

        // if too far or too close to enemy, deselect them
        if ((target.position - transform.position).magnitude < minDistance ||
            (target.position - transform.position).magnitude > maxDistance)
        {
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
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            // curDistance is the distance to the nearest enemy
            float curDistance = DistanceToTarget(go);

            if (curDistance < distance)
            {
                //Get's enemy model, used to aim at the emeny's center
                Vector3 goColliderCenter = GetTargetCenter(go);
                //check if we can see the enemy
                bool inLOS = RayCastToEnemy(go, goColliderCenter);

                Renderer goRenderer = go.GetComponentInChildren<Renderer>();
                if (goRenderer.isVisible && inLOS) //if in target is in view, set it as the closest target to be returned
                {
                    if (currentTarget) //get closest target to the right or left of the current one
                    {
                        Vector3 cross = Vector3.Cross(mainCamera.transform.forward, go.transform.position - currentTarget.position);
                        bool isCloser = (
                            targetDirection == TargetDirection.Right && cross.y > 0f ||
                            targetDirection == TargetDirection.Left && cross.y < 0f
                            );

                        if (isCloser)
                        {
                            closest = go;
                            targetColliderCenter = goColliderCenter;
                            distance = curDistance;
                        }
                    }
                    else //get closest target
                    {
                        closest = go;
                        targetColliderCenter = goColliderCenter;
                        distance = curDistance;
                    }
                }
            }
        }
        return closest;
    }

    /// Returns the distance from an enemy to the previous target if switching,
    /// or from the player if locking on for the first time.
    float DistanceToTarget(GameObject go)
    {
        if (currentTarget)
        {
            Vector3 targetDiff = go.transform.position - currentTarget.position;
            return targetDiff.magnitude;
        }
        else
        {
            Vector3 playerDiff = go.transform.position - transform.position;
            return playerDiff.magnitude;
        }
    }

    Vector3 GetTargetCenter(GameObject go)
    {
        CharacterController enemyCollision = go.GetComponent<CharacterController>();
        if (enemyCollision)
        {
            return enemyCollision.center * go.transform.localScale.y;
        }
        else
        {
            /*If enemy does not have a BoxCollider, use this as aim adjustment instead.*/
            return new Vector3(0, 0.2f, 0);
        }
    }

    /// Sends a Raycast to anemy transform to check if it is visible to the camera
    bool RayCastToEnemy(GameObject go, Vector3 goColliderCenter)
    {
        //Adjust the position so it aims at the center of the enemy model
        Vector3 dir = ((go.transform.position + goColliderCenter) - mainCamera.transform.position).normalized;
        Ray ray = new Ray(mainCamera.transform.position, dir);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, ~transparentObjects))
            return hit.collider.gameObject == go;

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}