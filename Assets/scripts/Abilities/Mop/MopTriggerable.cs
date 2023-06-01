using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using pState = States.PlayerStates;
using aState = States.ActionState;

public class MopTriggerable : PlayerAbility, IFrameCheckHandler
{
    [SerializeField] private Mop mop;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float stagger = 1f;
    [SerializeField] private float radius = 2f;
    //[SerializeField] private float delay = 0.5f;
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
        //NOT called
        playerForward = player.transform.forward;

        if (GameManager.instance.mana >= cost)
        {
            GameManager.instance.mana -= cost;
            GameManager.instance.updateManaAmount(GameManager.instance.mana);
            ActivateMop();
        }
    }

    public void onActiveFrameEnd()
    {
        // No need to do anything here
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
        // No need to do anything here
    }

    public override void Activate()
    {
        playerForward = player.transform.forward;
        clip.play();
        player.SetState(pState.Ability);
        state = aState.Inactionable;
        frameChecker.initCheck();
        frameChecker.checkFrames();

        //Audio Stuff
        playerObj = GameObject.Find("Player");
        audioSource = playerObj.GetComponentInChildren<AudioSource>();
        
        // Call the other functions manually
        onActiveFrameStart();
        onActiveFrameEnd();
        onAttackCancelFrameStart();
        onAttackCancelFrameEnd();
        onAllCancelFrameStart();
        onAllCancelFrameEnd();
        onLastFrameStart();
        onLastFrameEnd();
    }

    public override void Initialize(playerController player, Animator animator)
    {
        this.player = player;
        clip.animator = animator;
        clip.initialize();
        frameChecker.initialize(this, clip);

        if(GameManager.instance != null){
            cost = PersistentGameManager.instance.mopCost;
        }
    }

    private void ActivateMop()
    {
        Vector3 playerPosition = player.transform.position;
        Collider[] colliders = Physics.OverlapSphere(playerPosition, radius);
        
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy")){
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.isHit(damage, stagger);
                }
            }else if(collider.CompareTag("DustPile")){
                DustPile dustPile = collider.GetComponent<DustPile>();
                if (dustPile != null)
                {
                    dustPile.isHit(damage);
                }
            }
        }
    }
}
