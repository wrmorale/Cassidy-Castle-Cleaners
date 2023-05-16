using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using pState = States.PlayerStates;
using aState = States.ActionState;

public class BleachBombTriggerable : PlayerAbility, IFrameCheckHandler
{
    [SerializeField] private Bomb projectile;
    [HideInInspector] public Transform bulletSpawn;
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private float damage = 7f;
    [SerializeField] private float stagger = 20f;
    [SerializeField] public float cost;
    [SerializeField] private FrameParser clip;
    [SerializeField] private FrameChecker frameChecker;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float audioLevel;
    
    private playerController player;
    private Vector3 playerForward;
    private aState state;
    private GameObject playerObj;
    private AudioSource audioSource;
    public void onActiveFrameStart()
    {
        if(GameManager.instance.mana >= cost){
            GameManager.instance.mana -= cost;//mana reduced when using ability
            GameManager.instance.updateManaAmount(GameManager.instance.mana);
            SpawnProjectile(playerForward);
        }else{
            Debug.Log("Bleach Bomb: Not Enough Mana");
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
        Bomb clone = Instantiate(projectile, bulletSpawn.position, Quaternion.LookRotation(heading));
        clone.Initialize(speed, lifetime, damage, stagger, heading);
        clone.toTarget = player.toTargetPosition();
        clone.launch();
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
            cost = PersistentGameManager.instance.bleachBombCost;
        }
    }
}
