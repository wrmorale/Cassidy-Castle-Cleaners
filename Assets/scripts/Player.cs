using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    //this will keep track of stats for player
    [Header("stats")]
    [SerializeField]public float maxHealth;
    [SerializeField]public float health;
    [SerializeField]public float movementSpeed;
    [SerializeField]public float basicDamage;
    [SerializeField]public float staggerDamage;
    [SerializeField]public float attackSpeed;
    [SerializeField]public float cooldownReduction;
    
    public bool alive;
    public int lives;
    
    public List<Ability> abilities; 
    public Transform platform;
    public float fallLimit = -10; 
    
    //UI stuff
    /*public UIDocument hud;
    private HealthBar healthbar;
    
    [Range(0,1)]*/
    private PlayerHealthBar healthbar;
    public float healthPercent = 1;

    private Animator animator;
    private playerController playercontroller;
    private BroomAttackManager atkmanager;
    [SerializeField]
    private Image hurtpng;
    private Color color;

    [HideInInspector] public bool isInvulnerable; //This should be more clearly labeled as being i-frames for rolling
    public bool invincibleCheat = false;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        alive = true;
        isInvulnerable = false;

        /*
        var root = hud.rootVisualElement;
        healthbar = root.Q<HealthBar>();*/
        healthbar = GetComponentInChildren<PlayerHealthBar>();
        animator = GetComponentInChildren<Animator>();
        playercontroller = GetComponent<playerController>();
        atkmanager = GetComponent<BroomAttackManager>();
        healthbar.setMaxHealth(healthPercent);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < platform.position.y + fallLimit){
            health = 0;
            alive = false;
        }
        if(hurtpng.color.a > 0){
            color = hurtpng.color;
            color.a -= 0.01f;
            hurtpng.color = color;
        }
    }

    public void isHit(float damage){
        //print("Player took " + damage + " damage");
        
        if(!isInvulnerable && !invincibleCheat){
            
            
            health -= damage;
            health = Mathf.Clamp(health, 0 , maxHealth);
            healthPercent = health / maxHealth;
            healthbar.setHealth(healthPercent);
            //if(health >= 1){
                //StartCoroutine(HandleDamage());
                color = hurtpng.color;
                color.a = 1f;
                hurtpng.color = color;
            //}
            if(health <= 0){
                alive = false;
            }
        }
        
    }

    /*
    IEnumerator HandleDamage(){
        playercontroller.SetState(States.PlayerStates.Damaged);
        
        animator.SetBool("Falling", false); 
        animator.SetBool("Jumping", false);
        animator.SetBool("Walking", false);      
        animator.SetBool("Running", false);
        animator.SetBool("Attacking", false);
        animator.SetBool("Rolling", false);
        
        atkmanager.SetWeaponCollider(false);
        atkmanager.combo = 0;
        atkmanager.activeClip.animator.SetInteger("Combo", 0);
        isInvulnerable = false;
        //animator.SetTrigger("Damaged");
        color = hurtpng.color;
        color.a = 1f;
        hurtpng.color = color;
        
        yield return new WaitForSeconds(0.5f);
        //animator.SetTrigger("Recovery");
        playercontroller.SetState(States.PlayerStates.Idle);
    }
    */
}
