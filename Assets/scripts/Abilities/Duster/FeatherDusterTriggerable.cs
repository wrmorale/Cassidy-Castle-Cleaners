using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using pState = States.PlayerStates;
using aState = States.ActionState;

public class FeatherDusterTriggerable : PlayerAbility, IFrameCheckHandler
{
    [SerializeField] private Projectile projectile;
    [HideInInspector] public Transform bulletSpawn;
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private float damage = 7f;
    [SerializeField] private float stagger = 1f;
    [SerializeField] private float spread = 120f;
    [SerializeField] private int projectileCount = 3;
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
        /*Adjust throw direction according to the locked target's current position.
         Done here so that the projectile aim can be adjusted right as they spawn, rather
        than when the animation starts.*/
        if (player.toTargetPosition() != Vector3.zero)
        {
            //Best point to target is probably the enemy's model
            //Apparently this IS the world space of the transform, so why is it below the map and not where the model actually is?
            //Transform of enemy = bottom of their collision box. Model position is somehow the same despite what the Unity editor shows
            playerForward = ((player.targetLock.currentTarget.position + player.targetLock.targetColliderCenter) - bulletSpawn.position).normalized;
            Debug.DrawLine(bulletSpawn.position, (player.targetLock.currentTarget.position + player.targetLock.targetColliderCenter), Color.white, 3.0f); //Only visible with Gizmos >:(
        }
        if(GameManager.instance.mana >= cost){
            GameManager.instance.mana -= cost;//mana reduced when using ability
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
        Projectile clone = Instantiate(projectile, bulletSpawn.position, Quaternion.LookRotation(heading));
        clone.Initialize(speed, lifetime, damage, stagger, heading, 0);
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
        
        for (int i = 0; i < projectileCount; i++)
        {
            float theta = ((float)i).map(0, projectileCount - 1, -(spread / 2), spread / 2);
            Vector3 heading = Quaternion.Euler(0, -theta, 0) * playerForward;
            SpawnProjectile(heading);
            audioSource.PlayOneShot(audioClip, audioLevel);
            yield return new WaitForSeconds(firerate);
        }
    }
}