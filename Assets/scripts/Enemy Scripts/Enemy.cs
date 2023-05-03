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
    private EnemyHealthBar enemyHealthBar;
    private float HealthPercent = 1;

    [Header("Movement info")]
    [SerializeField] public float movementSpeed;
    public float rotationSpeed;
    [SerializeField] public float idleMovementRange;
    [HideInInspector] 
    public Vector3 movement;
    public Vector3 moveHistory; //Used for rotating towards movement node
    public Vector3 gravityBuildup;
    public float gravity = -9.81f;


    [HideInInspector]
    public CharacterController enemyController;
    public Rigidbody playerBody;

    [Header("Animator info")]
    //public Animator animator;
    [HideInInspector] public AnimatorStateInfo stateInfo;

    [Header("Dust Piles info")]
    public float detectionRange = 5.0f;
    public float healingSpeed = 0.1f;
    public GameObject dustPilePrefab;
    public int maxDustPiles = 5;
    private List<DustPile> dustPiles = new List<DustPile>();

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
        foreach (DustPile dustPile in dustPiles) {
            if (dustPile.health < dustPile.maxHealth) {
                float distance = Vector3.Distance(transform.position, dustPile.transform.position);
                if (distance <= detectionRange) {//check if dust pile doesn't have full health and is nearby
                    dustPile.IncreaseHealth(healingSpeed * Time.deltaTime);
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
        if (!enemyController.isGrounded)
            gravityBuildup.y += gravity * Time.fixedDeltaTime;
        else
            gravityBuildup = Vector3.zero;
        //Remember to use Time.fixedDeltatime in functions that affect movement
        enemyController.Move(movement + (gravityBuildup * Time.fixedDeltaTime));
        moveHistory = movement;
        movement = Vector3.zero;
    }

    //Changed to virtual so that boss mirrors can override this
    public virtual void isHit(float damage){
        //decrease health
        currentHealth -= damage;
        //damageFlash.FlashStart();
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        //Update health bar
        HealthPercent = currentHealth / maxHealth;
        enemyHealthBar.setHealth(HealthPercent);

        /*To add: Staggering*/

        //Died?
        if(currentHealth <= 0){
            /*To add: Replace this Destory with a behavior tree interrupt*/
            Destroy(gameObject);
        }
    }
}
