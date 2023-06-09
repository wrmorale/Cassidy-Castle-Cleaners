using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using Extensions;
using States;

/*** This code is based on sample unity movement API code. ***/

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class playerController : MonoBehaviour, IFrameCheckHandler
{
    [SerializeField] public float playerSpeed = 2.5f;
    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float walkThreshold = 0.5f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] public float turnSmoothTime = 0.0f;
    [SerializeField] private FrameParser jumpClip;
    [SerializeField] private FrameChecker jumpFrameChecker;

    [SerializeField] public PlayerAbility[] playerAbilities = new PlayerAbility[4];

    private float turnSmoothVelocity;

    [HideInInspector]
    public CharacterController controller;
    private PlayerInput playerInput;
    private BroomAttackManager attackManager;
    public TargetLock targetLock; /*Added for lock-on improvements*/
    public Player playerStats;


    [HideInInspector]
    public PlayerAbility activeAbility;
    private GameObject model;
    private GameObject metarig;
    private GameObject hip;
    private Vector3 playerVelocity;
    private float lastY;
    private float lastRootY;
    private bool groundedPlayer;
    private bool inJumpsquat = false;
    public bool castingAllowed = true;
    public bool isCasting = false;

    private Transform cam;

    [HideInInspector]
    public InputAction moveAction;
    public InputAction walkAction;
    public InputAction jumpAction;
    public InputAction attackAction;

    [HideInInspector]
    public InputAction[] abilityActions;

    [HideInInspector]
    public int channeledAbility;

    public States.PlayerStates state;

    //Animation stuff
    Animator animator;

    // jump framedata management
    public void onActiveFrameStart()
    {
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    }

    public void onActiveFrameEnd()
    {
        inJumpsquat = false;
    }

    public void onAttackCancelFrameStart() { }

    public void onAttackCancelFrameEnd() { }

    public void onAllCancelFrameStart() { }

    public void onAllCancelFrameEnd() { }

    public void onLastFrameStart() { }

    public void onLastFrameEnd() { }

    public void updateMe(float time) { }

    private void Awake()
    {
        controller = gameObject.GetComponent<CharacterController>();
        playerInput = gameObject.GetComponent<PlayerInput>();
        animator = gameObject.GetComponentInChildren<Animator>();
        model = transform.Find("maid68").gameObject;
        metarig = transform.Find("maid68/metarig").gameObject;
        hip = transform.Find("maid68/metarig/hip").gameObject;
        attackManager = gameObject.GetComponent<BroomAttackManager>();
        cam = Camera.main.transform;
        lastRootY = hip.transform.localPosition.y;
        // add actions from playerControls here
        moveAction = playerInput.actions["Run"];
        jumpAction = playerInput.actions["Jump"];
        attackAction = playerInput.actions["Attack"];
        walkAction = playerInput.actions["Walk"];
        abilityActions = new InputAction[]
        {
            playerInput.actions["Ability_1"],
            playerInput.actions["Ability_2"],
            playerInput.actions["Ability_3"],
            playerInput.actions["Ability_4"],
            playerInput.actions["Roll"]
        };

        for (int i = 0; i < playerAbilities.Length; i++)
        {
            if (playerAbilities[i] != null)
            {
                playerAbilities[i].Initialize(this, animator);
            }
        }
        jumpClip.initialize();
        jumpFrameChecker.initialize(this, jumpClip);
        SetState(States.PlayerStates.Idle);

        targetLock = gameObject.GetComponent<TargetLock>();
        playerStats = gameObject.GetComponent<Player>();
    }

    void Update()
    {
        // add gravity
        ApplyGravity();

        // update playercharacter values
        lastY = controller.transform.position.y;
        jumpFrameChecker.checkFrames();
        groundedPlayer = controller.isGrounded;

        // store ability pressed, if any
        channeledAbility = ParseAbilityInput();

        if (groundedPlayer)
        {
            StopFalling();
        }

        if (state != null && state == States.PlayerStates.Ability)
        {
            activeAbility.updateMe(Time.deltaTime);
        }

        if (state == States.PlayerStates.Attacking)
        {
            attackManager.updateMe(Time.deltaTime);
        }

        if (state != States.PlayerStates.Attacking && state != States.PlayerStates.Ability && state 
            != States.PlayerStates.Dead && state != States.PlayerStates.Damaged && state != States.PlayerStates.Talking)
        {
            SetState(States.PlayerStates.Idle);
            model.transform.localPosition = Vector3.zero;

            HandleMovement();

            // animate falling player
            if (controller.transform.position.y <= lastY && !groundedPlayer && !inJumpsquat)
            {
                animator.SetBool("Jumping", false);
                animator.SetBool("Falling", true);
            }

            // Changes the height position of the player
            if (jumpAction.triggered)
            {
                Jump();
            }

            if (attackAction.triggered)
            {
                Attack();
            }
            else if (channeledAbility >= 0 && !inJumpsquat && castingAllowed)
            {
                ActivateAbility(channeledAbility);
            }
        }
        

        // cycle cooldowns
        ManageCooldowns(Time.deltaTime);
    }
    public void HandleDeath()
    {
        SetState(States.PlayerStates.Dead);
        animator.SetTrigger("Death");

    }

    private void ApplyGravity()
    {
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void StopFalling()
    {
        // handle edge case where player lands without ever gaining downward velocity
        if (!inJumpsquat)
        {
            animator.SetBool("Jumping", false);
        }

        // stop falling animation on land
        if (playerVelocity.y < 0)
        {
            playerVelocity.y = -2.0f;
            animator.SetBool("Falling", false);
        }
    }

    private void HandleMovement()
    {
        // store direction input
        Vector2 input = moveAction.ReadValue<Vector2>();

        if (input.x != 0 || input.y != 0)
        { // if there is movement input
            bool walking = false;


            Vector3 move = new Vector3(input.x, 0, input.y);
            if (move.magnitude < walkThreshold || walkAction.triggered)
            {
                walking = true;
            }

            // store calculated model rotation
            move = RotatePlayer(input);

            if (walking)
            {
                controller.Move(move * Time.deltaTime * walkSpeed);
                animator.SetBool("Walking", true);
                animator.SetBool("Running", false);
            }
            else
            {
                controller.Move(move * Time.deltaTime * playerSpeed);
                animator.SetBool("Running", true);
                animator.SetBool("Walking", false);
            }
        }
        else
        {
            animator.SetBool("Running", false);
            animator.SetBool("Walking", false);
        }
    }

    public void SetState(States.PlayerStates newState)
    {
        state = newState;
    }

    public void Jump()
    {
        if (groundedPlayer && state != States.PlayerStates.Jumping)
        {
            SetState(States.PlayerStates.Jumping);
            animator.SetBool("Jumping", true);
            inJumpsquat = true;
            jumpFrameChecker.initCheck();
            animator.Play("jump", 0);
        }
    }

    private void Attack()
    {
        if (!inJumpsquat)
        {
            // log current root bone position
            lastRootY = hip.transform.localPosition.y;
            //set state to attacking
            SetState(States.PlayerStates.Attacking);
            // launch animations and attacks
            attackManager.handleAttacks();
        }
    }

    public void ActivateAbility(int idx)
    {
        if (targetLock.currentTarget && channeledAbility != 1) //To Do: Make roll not an ability so that this exception doesn't cause problem
        { //Face lock-on target if locked on

            Quaternion newRotation = Quaternion.LookRotation(toTargetPosition(), controller.transform.up);
            newRotation = Quaternion.Euler(0, newRotation.eulerAngles.y, 0);
            controller.transform.rotation = newRotation;
            
            //Could also make it adjust to height as well...
        }

        SetState(States.PlayerStates.Ability);
        activeAbility = playerAbilities[idx];
        activeAbility.Activate();
        if(channeledAbility != 1){
            StartCoroutine(castingTimer());
        }
        
    }

    IEnumerator castingTimer()
    {
        yield return new WaitForSeconds(0.5f);
        isCasting = true;
    }

    //Returns a vector from player to the current lock-on target
    public Vector3 toTargetPosition()
    {
        if (targetLock.currentTarget)
            return targetLock.currentTarget.position - transform.position; //Should replace transform with bullet spawn position
        else
            return Vector3.zero;
    }

    public Vector3 RotatePlayer(Vector2 input)
    {
        // calculate model rotation
        float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cam.eulerAngles.y; // from front facing position to direction pressed + camera angle.
        float angle = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            targetAngle,
            ref turnSmoothVelocity,
            turnSmoothTime
        );
        transform.rotation = Quaternion.Euler(0f, angle, 0f); // apply rotation

        // move according to calculated target angle
        return Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
    }

    // Simulates root motion when called during an animation.
    public void MoveRoot()
    {
        if (lastRootY != hip.transform.localPosition.y)
        {
            float diff = lastRootY - hip.transform.localPosition.y;
            controller.Move(
                transform.forward * diff * metarig.transform.localScale.y * transform.localScale.z
            );
            model.transform.localPosition =
                model.transform.localPosition
                + (Vector3.forward * -diff * metarig.transform.localScale.y);
        }
        lastRootY = hip.transform.localPosition.y;
    }

    // Call when moving from one state that applies root motion to any state other than idle.
    public void ResetRoot()
    {
        lastRootY = 0;
        model.transform.localPosition = Vector3.zero;
    }

    /**
     * Cycles player ability cooldowns when called each frame.
     *
     * @param float time: Current time elapsed since run started.
     */
    public void ManageCooldowns(float time)
    {
        for (int i = 0; i < playerAbilities.Length; i++)
        {
            if (playerAbilities[i] != null)
            {
                playerAbilities[i].UpdateCooldown(time);
            }
        }
    }

    // Returns the index of the first ability that was input and available to fire, -1 if none are input or available.
    public int ParseAbilityInput()
    {
        for (int i = 0; i < playerAbilities.Length; i++)
        {
            if (abilityActions[i].triggered && playerAbilities[i] != null)
            {
                if (playerAbilities[i].IsReady())
                {
                    return i;
                }
            }
        }
        return -1;
    }
}
