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
    public float projectileAttackDuration = 10.0f;

    /*I should probably experiment a bit with the golem before I give the others their tasks...*/
    [Header("Aggro Status")]
    public bool aggro = false;
    public Transform player;
    [HideInInspector] public int phase; //1 = phase 1, 2 = spawn enemies, 3 = final phase, 4+ = defeated

    public bool isCoroutineRunning = false;

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
    
    public IEnumerator projectileAttack(float duration, float projectilesPerSec) {
        Projectile projectile = currPosessedMirror.projectilePrefab; 
        float secondsPerProjectile = 1.0f / projectilesPerSec; // Calculate the time between each projectile
        float projectileTimer = secondsPerProjectile; // Tracks time since last projectile was fired
        float elapsedTime = 0.0f; // Tracks time since the attack started
        while (elapsedTime < duration) {
            foreach (MirrorBossMirror mirror in mirrors) {
                Animator anim = mirror.GetComponentInChildren<Animator>();
                anim.SetBool("Shooting", true);
                // Instantiate a clone of the projectile prefab at the mirror's position and rotation
                Projectile projectileClone = Instantiate(projectile, mirror.transform.position, mirror.transform.rotation);
                Vector3 direction = mirror.transform.right;
                // Activate and Initialize the clone
                projectileClone.gameObject.SetActive(true);
                projectileClone.Initialize(mirror.projectileSpeed, mirror.projectileLifetime, mirror.projectileDamage, 1f, direction);
            }
            yield return new WaitForSeconds(secondsPerProjectile); // Pause the coroutine until the next frame
            elapsedTime += secondsPerProjectile; // Increment the elapsed time
        }
        isCoroutineRunning = false;
        foreach (MirrorBossMirror mirror in mirrors) {
            Animator anim = mirror.GetComponentInChildren<Animator>();
            anim.SetBool("Shooting", false);
        }
    }
}
