using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using pState = States.PlayerStates;
using aState = States.ActionState;
using UnityEngine.SceneManagement;

public class MopTriggerable : PlayerAbility, IFrameCheckHandler
{
    [SerializeField] private Mop mop;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float stagger = 1f;
    [SerializeField] private float radius = 1f;
    [SerializeField] private float mopRotationSpeed = 360f; // Degrees per second
    [SerializeField] public float cost;
    [SerializeField] private FrameParser clip;
    [SerializeField] private FrameChecker frameChecker;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private AudioClip noManaClip;
    [SerializeField] private float audioLevel;

    private playerController player;
    private aState state;
    private bool mopActive;
    private float mopRotationTimer;
    private bool mopSpawned;
    private GameObject mopObject;
    private GameObject playerObj;
    private AudioSource audioSource;

    public void onActiveFrameStart()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if ((!mopSpawned && GameManager.instance.mana >= cost) && (sceneName == "room_4" || sceneName == "NewBossScene"))
        {
            GameManager.instance.mana -= cost;
            GameManager.instance.updateManaAmount(GameManager.instance.mana);
            ActivateMop();
            mopSpawned = true;
        }
        else{
            audioSource.PlayOneShot(noManaClip, audioLevel);
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
        if (state == aState.AttackCancelable)
        {
            state = aState.Inactionable;
        }
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
        //Nothing
    }

    public override void Activate()
    {
        clip.play();
        player.SetState(pState.Ability);
        state = aState.Inactionable;
        frameChecker.initCheck();
        frameChecker.checkFrames();

        mopActive = true;
        mopRotationTimer = 0f;
        mopSpawned = false;

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

        if (GameManager.instance != null)
        {
            cost = PersistentGameManager.instance.mopCost;
        }
    }

    private void RotateMop()
    {
        mopRotationTimer += Time.deltaTime;
        float angle = mopRotationSpeed * Time.deltaTime;
        Vector3 newPos = player.transform.position + Quaternion.Euler(0f, angle, 0f) * (mopObject.transform.position - player.transform.position);
        newPos.y = player.transform.position.y;
        mopObject.transform.position = newPos;

        // If the mop has completed a full rotation, destroy it
        if (mopRotationTimer >= 360f)
        {
            mopActive = false;
            Destroy(mopObject);
        }
    }

    private void ActivateMop()
    {
        Vector3 playerPosition = player.transform.position;
        Vector3 spawnPosition = playerPosition + player.transform.forward * 0.5f;
        Vector3 initialOffset = new Vector3(0f, 0.25f, 0f);

        mopObject = Instantiate(mop.gameObject, spawnPosition + initialOffset, Quaternion.identity);
        mopObject.transform.position = spawnPosition + initialOffset;
        mopObject.gameObject.SetActive(true);

        // Perform damage checks against enemies and dust piles
        Collider[] colliders = Physics.OverlapSphere(playerPosition, radius);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.isHit(damage, stagger);
                }
            }
            else if (collider.CompareTag("DustPile"))
            {
                DustPile dustPile = collider.GetComponent<DustPile>();
                if (dustPile != null)
                {
                    dustPile.isHit(damage);
                }
            }
        }

        // Disable the mop's collider to prevent unintended triggers during orbit
        Collider mopCollider = mopObject.GetComponent<Collider>();
        if (mopCollider != null)
        {
            mopCollider.enabled = false;
        }

        // Invoke the OrbitMop method repeatedly with a delay to simulate the orbit behavior
        InvokeRepeating("OrbitMop", 0f, Time.deltaTime);
    }

    private void OrbitMop()
    {
        float orbitSpeed = 360f * 1.5f; // Degrees per second

        // Calculate the new mop position based on the orbit parameters
        float angle = orbitSpeed * Time.deltaTime;
        Vector3 newPosition = mopObject.transform.position;
        newPosition = Quaternion.Euler(0f, angle, 0f) * (newPosition - player.transform.position);
        newPosition += player.transform.position;

        // Update the mop's position
        mopObject.transform.position = newPosition;
        mopObject.transform.rotation = Quaternion.Euler(0f, player.transform.eulerAngles.y + mopRotationTimer*1f - 90f, 90f);

        // Increment the mopRotationTimer
        mopRotationTimer += angle;

        // If the mop has completed a full rotation, destroy it
        if (mopRotationTimer >= 360f)
        {
            mopActive = false;
            Destroy(mopObject);
            CancelInvoke("OrbitMop");
        }
    }
}
