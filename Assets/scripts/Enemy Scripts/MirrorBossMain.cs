using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class MirrorBossMain : MonoBehaviour //Will derive from Enemy class later
{
    //When changing mirrors, will need to change the body collider to be the corresponding mirror

    public List<MirrorBossMirror> mirrors;
    [HideInInspector] public MirrorBossMirror currPosessedMirror;
    int currMirrorIndex;
    public bool canBeHarmed = false; //Starts immune to damage during the starting cutscene, though I suppose we could just make it that none of the mirrors are posessed at first.
    BehaviourTreeRunner btRunner;

    [Header("Stats")]
    [SerializeField] public string enemyName;
    [SerializeField] public float maxHealth = 1.0f; //Shouldn't these be integers?
    float currentHealth;
    float healthPercent = 1.0f;

    /*I should probably experiment a bit with the golem before I give the others their tasks...*/
    [Header("Aggro Status")]
    public bool aggro = false;
    public Transform player;
    [HideInInspector] public int phase; //1 = phase 1, 2 = spawn enemies, 3 = final phase, 4+ = defeated

    // Start is called before the first frame update
    void Start()
    {
        currPosessedMirror = mirrors[0];
        currMirrorIndex = 0;
        btRunner = GetComponentInChildren<BehaviourTreeRunner>();
        phase = 1;

        //Remove later
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Before calling this function we will play an animation that makes the entity "vanish" from the current mirror
    public void PosessMirror(int mirrorIndex)
    {
        foreach(MirrorBossMirror mirror in mirrors) //That's a mouthful
        {
            mirror.SetPossessed(false);
        }
        mirrors[mirrorIndex].SetPossessed(true);
        currPosessedMirror = mirrors[mirrorIndex];
        currMirrorIndex = mirrorIndex;
        Debug.Log("Posessed mirror " + mirrorIndex);

        //Change the context?
        Context newContext = Context.CreateFromGameObject(currPosessedMirror.gameObject);
        btRunner.tree.Bind(newContext);

        //Then for the mirror that just became active, we play an animation for the entity appearing.
    }

    public void StopPosessingMirrors()
    {
        currPosessedMirror.SetPossessed(false);
        currMirrorIndex = -1;
    }

    //Randomly choose a mirror to posess besides the one that is already posessed
    public void PosessMirrorRandom()
    {
        List<int> mirrorIndicies = new List<int>();
        for(int i = 0; i < mirrors.Count; i++)
        {
            if(i != currMirrorIndex)
            {
                mirrorIndicies.Add(i);
            }
        }

        int choice = (Random.Range(0, mirrorIndicies.Count));
        PosessMirror(mirrorIndicies[choice]);
    }

    //Remove later
    //Mirrors count as enemies. When they are hit, instead of doing usual enemy stuff,
    //they call this method.
    public void isHit(float damage)
    {
        if (canBeHarmed)
        {
            currentHealth -= damage;
            //damageFlash.FlashStart();
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            healthPercent = currentHealth / maxHealth;
            //enemyHealthBar.setHealth(healthPercent);

            //Proceed to phase 1.5 at 2/3 health
            if(healthPercent <= 0.34 && phase == 1)
            {
                Debug.Log("Proceeding to phase 2");
                Debug.Log("Boss health: " + currentHealth);
                btRunner.tree.rootNode.Abort();
                canBeHarmed = false;
                phase += 1;
            }

            if (currentHealth <= 0)
            {
                Debug.Log("Boss defeated??");
                // Destroy the cube when it has no health left
                //this should work for death animation but not all enemies have one so it gets errors
                //animator.SetBool("Death", true);
                //StartCoroutine(waitForAnimation("Death"));
                //Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogWarning("Boss cannot be hurt right now!");
        }
    }

    //Starts the projectile attack
    public void projectileAttack(){
        float abilityDuration = 5.0f; //temp ability duration, this could be set to the animation time
        float timeSinceLastProjectile = 0.0f; // Tracks time since last projectile was fired
        float projectilesPerSec = 2;  //temporary attack speed
        float secondsPerProjectile = 1.0f / projectilesPerSec; // Calculate the time between each projectile
        GameObject projectilePrefab = Resources.Load<GameObject>("Projectile");
        while(abilityDuration >= 0.0f){
            abilityDuration -= Time.deltaTime;
            timeSinceLastProjectile += Time.deltaTime;
            if (timeSinceLastProjectile >= secondsPerProjectile) {
                foreach(MirrorBossMirror mirror in mirrors){
                    // Instantiate a clone of the projectile prefab at the mirror's position and rotation
                    GameObject projectile = Instantiate(projectilePrefab, mirror.transform.position, mirror.transform.rotation);
                    // Activate the clone
                    projectile.SetActive(true);
                }
                // Reset the time since the last projectile was fired
                timeSinceLastProjectile -= secondsPerProjectile;
            }
        }
        /*while(abilityDuration >= 0.0){
            abilityDuration -= Time.deltaTime;
            foreach(MirrorBossMirror mirror in mirrors){
                float projectilesPerSec = 2; //temporary attack speed
                //Make projectiles spawn at the rate of the projectilesPerSec
                    
                // projectiles spawn pattern
            }
        }*/
        
    }
}
