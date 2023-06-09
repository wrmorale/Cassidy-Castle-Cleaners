using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using pState = States.PlayerStates;
using aState = States.ActionState;
using UnityEngine.SceneManagement;

public class SoapBarTriggerable : PlayerAbility, IFrameCheckHandler
{
    [SerializeField] private Projectile projectile;
    [HideInInspector] public Transform bulletSpawn;
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private float lifetime = 1f;
    [SerializeField] public float damage = 7f;
    [SerializeField] private float stagger = 1f;
    [SerializeField] private float firerate = .1f;
    [SerializeField] public float cost;
    [SerializeField] private FrameParser clip;
    [SerializeField] private FrameChecker frameChecker;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private AudioClip noManaClip;
    [SerializeField] private float audioLevel;
    
    private playerController player;
    private Vector3 playerForward;
    private aState state;
    private GameObject playerObj;
    private AudioSource audioSource;
    public void onActiveFrameStart()
    {
        playerForward = player.transform.forward; // Set playerForward to player's forward direction
        string sceneName = SceneManager.GetActiveScene().name;

        if((GameManager.instance.mana >= cost) && (sceneName == "room_2" || sceneName == "room_3" || sceneName == "room_4" || sceneName == "NewBossScene")){
            GameManager.instance.mana -= cost;
            GameManager.instance.updateManaAmount(GameManager.instance.mana);
            player.StartCoroutine(Fire());
        }else{
            audioSource.PlayOneShot(noManaClip, audioLevel);
        }
    }
    public void onActiveFrameEnd()
    {
    }
    public void onAttackCancelFrameStart()
    {
        state = aState.AttackCancelable;
    }
    public void onAttackCancelFrameEnd()
    {
        if (state == aState.AttackCancelable) { state = aState.Inactionable; }
    }
    public void onAllCancelFrameStart()
    {
        state = aState.AllCancelable;
    }
    public void onAllCancelFrameEnd()
    {
        state = aState.Inactionable;
    }
    public void onLastFrameStart()
    {
    }
    public void onLastFrameEnd()
    {
        player.SetState(States.PlayerStates.Idle);
    }


    public override void updateMe(float time) 
    {
        frameChecker.checkFrames();

        // if we are playing a different animation than this ability, change the player state
        // This avoids hard locking the player.
        if (!clip.animator.GetCurrentAnimatorStateInfo(0).IsName(clip.animatorStateName) &&
            player.state == pState.Ability)
        {
            player.SetState(States.PlayerStates.Idle);
            // Animation has ended, we should be out of the Ability State
        }
    }
    public override void Activate()
    {
        playerForward = player.transform.forward;
        clip.play();
        player.SetState(pState.Ability);
        state = aState.Inactionable;
        frameChecker.initCheck();
        frameChecker.checkFrames();
        cooldownTimer = baseCooldown;

        //Audio Stuff
        playerObj = GameObject.Find("Player");
        audioSource = playerObj.GetComponentInChildren<AudioSource>();
    }

    public void SpawnProjectile(Vector3 heading)
    {
        float yOffset = 0.22f; // Adjust this value to control the vertical offset
        Vector3 spawnPosition = bulletSpawn.position + new Vector3(0f, -yOffset, 0f);

        Projectile clone = Instantiate(projectile, spawnPosition, Quaternion.LookRotation(heading));
        clone.Initialize(speed, lifetime, damage, stagger, heading, 0);

        // Disable gravity for the projectile
        Rigidbody rb = clone.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
        }
    }

    public override void Initialize(playerController player, Animator animator) 
    {
        this.player = player;
        clip.animator = animator;
        clip.initialize();
        frameChecker.initialize(this, clip);
        bulletSpawn = player.transform.Find("maid68/metarig/hip/spine/chest/shoulder.R/upper_arm.R/forearm.R/hand.R");

        // Set the cost based on the value of dusterCost in GameManager
        if (GameManager.instance != null)
        {
            cost = PersistentGameManager.instance.dusterCost;
        }
    }

    IEnumerator Fire()
    {
        // Use playerForward directly as the heading
        Vector3 heading = playerForward;
        SpawnProjectile(heading);
        audioSource.PlayOneShot(audioClip, audioLevel);
        yield return new WaitForSeconds(firerate);
    }
}