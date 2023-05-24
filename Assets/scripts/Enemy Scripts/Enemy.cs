using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DamageFlash;
using States;
using Extensions;
using TheKiwiCoder;

public class Enemy : MonoBehaviour
{
    [Header("Basic Stats")]
    [SerializeField]public string enemyName;
    [SerializeField]public float maxHealth; //Shouldn't these be integers?
    [HideInInspector][SerializeField]public float currentHealth;
    [SerializeField]public float aggroRange;
    [SerializeField]public float maxStaggerAmount;
    [HideInInspector] public float currentStaggerAmount = 0;
    [HideInInspector] public bool isStaggered = false;
    [HideInInspector] public bool isDead = false;
    public GameObject onDeathPoof;
    public Transform poofSpawnTransform;
    private EnemyHealthBar enemyHealthBar;
    private float HealthPercent = 1;

    [Header("Movement info")]
    [SerializeField] public float movementSpeed;
    public float rotationSpeed;
    [SerializeField] public float idleMovementRange;


    [HideInInspector] public Vector3 movement;
    [HideInInspector] public Vector3 moveHistory; //Used for rotating towards movement node
    [HideInInspector] public Vector3 gravityBuildup;
    public float gravity = -9.81f;


    [HideInInspector] public CharacterController enemyController;
    [HideInInspector] public Rigidbody playerBody;

    [Header("Animator info")]
    //public Animator animator;
    [HideInInspector] public AnimatorStateInfo stateInfo;

    [Header("Dust Piles info")]
    public float detectionRange = 5.0f;
    public float healingSpeed = 0.1f;
    public GameObject dustPilePrefab;
    public int maxDustPiles = 5;
    private List<DustPile> dustPiles = new List<DustPile>();

    [Header("Behaviour Tree info")]
    BehaviourTreeRunner BTrunner;

    //GameObject damageFlashObject;
    void Start(){
        enemyController = GetComponent<CharacterController>();
        playerBody = FindObjectOfType<Player>().GetComponent<Rigidbody>(); //Should find player automatically now
        enemyHealthBar = GetComponentInChildren<EnemyHealthBar>();
        currentHealth = maxHealth;
        enemyHealthBar.setMaxHealth(HealthPercent);
        BTrunner = GetComponent<BehaviourTreeRunner>();
        gravityBuildup = Vector3.zero;
        //damageFlashObject = Instantiate(damageFlashPrefab, transform.position, Quaternion.identity);
        //damageFlash = damageFlashObject.GetComponent<DamageFlash>();
    }

    void Update(){
        /*BIG PROBLEM: Update stops working entirely if there are errors in the game manager*/

        // Check for nearby dust piles that need healing
        if (!isDead)
        {
            foreach (DustPile dustPile in dustPiles)
            {
                if (dustPile.health < dustPile.maxHealth)
                {
                    float distance = Vector3.Distance(transform.position, dustPile.transform.position);
                    if (distance <= detectionRange)
                    {//check if dust pile doesn't have full health and is nearby
                        dustPile.IncreaseHealth(healingSpeed * Time.deltaTime);
                    }
                }
            }
        }

        // Check if we need to generate a new dust pile
        if (dustPiles.Count < maxDustPiles) {
            GameObject newDustPile = Instantiate(dustPilePrefab, transform.position + new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)), Quaternion.identity);
            DustPile newDustPileScript = newDustPile.GetComponent<DustPile>();
            newDustPileScript.SetHealth(0.1f); // set a low starting health
            dustPiles.Add(newDustPileScript);
        }
    }

    private void FixedUpdate()
    {
        if (enemyController)
        {
            if (!enemyController.isGrounded)
                gravityBuildup.y += gravity * Time.fixedDeltaTime;
            else
                gravityBuildup = Vector3.zero;
            //Remember to NOT use Time.fixedDeltatime in functions that affect movement
            enemyController.Move((movement + gravityBuildup) * Time.fixedDeltaTime);
            moveHistory = movement;
            movement = Vector3.zero;
        }
    }

    //Changed to virtual so that boss mirrors can override this
    public virtual void isHit(float damage, float staggerDamage = 1.0f){
        //decrease health
        currentHealth -= damage;
        //damageFlash.FlashStart();
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        //Update health bar
        HealthPercent = currentHealth / maxHealth;
        enemyHealthBar.setHealth(HealthPercent);

        //Died?
        if (!isDead)
        {
            if (currentHealth <= 0)
            {
                isDead = true; //Heavy is dead!
                BTrunner.tree.rootNode.Abort();
                gameObject.tag = "Untagged"; //Makes them unable to be targeted
                //However if changing the enemy tag causes problems, we can instead use GetComponent to check if the enemy is dead in TargetLock.cs
                return;
            }

            //Staggering
            if (maxStaggerAmount > 0)
            { //for bigger enemies
                currentStaggerAmount += staggerDamage;
                if (currentStaggerAmount >= maxStaggerAmount && isStaggered == false)
                {
                    isStaggered = true;
                    //do the BT interupt
                    BTrunner.tree.rootNode.Abort();
                    //stagger animation done in BT
                    //reset stagger done in BT
                }
            }
            else if (staggerDamage > 0)
            {//smaller enemies
                isStaggered = true;
                BTrunner.tree.rootNode.Abort();
                //Always play flinch animation from start, even if the enemy is already flinching
                //(As long as the attack actually does stagger damage
            }
        }
    }

    //Called by Behavior tree after the death animation
    public void RemoveEnemy()
    {
        Vector3 poofSpawnPos = transform.position + (Vector3.up * 0.1f);
        Destroy(gameObject);
        if (onDeathPoof && poofSpawnTransform)
        {
            GameObject deathPoof = Instantiate(onDeathPoof);
            deathPoof.transform.position = poofSpawnPos;
        }
    }
}
