using System;
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
    public float currentHealth;
    float healthPercent = 1.0f;

    [Header("Projectile Stats")]
    [SerializeField] float projectileAttackDuration = 5.0f;
    [SerializeField] float projectilesPerSec = 3.0f;
    [SerializeField] float projectileMaxAngle = 25.0f;

    [Header("Enemy Spawns")]
    [SerializeField] GameObject spawnArea;
    public GameObject enemyPrefab;

    /*I should probably experiment a bit with the golem before I give the others their tasks...*/
    [Header("Aggro Status")]
    public bool aggro = false;
    public Transform player;
    public int phase; //1 = phase 1, 2 = spawn enemies, 3 = final phase, 4+ = defeated

    public bool isCoroutineRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        currPosessedMirror = mirrors[0];
        currMirrorIndex = 0;
        btRunner = GetComponentInChildren<BehaviourTreeRunner>();
        phase = 1;

        //Remove later
        canBeHarmed = true;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(phase == 2){
            phase2CompletionCheck();
        }
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
        //Debug.Log("Posessed mirror " + mirrorIndex);

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

        int choice = (UnityEngine.Random.Range(0, mirrorIndicies.Count));
        PosessMirror(mirrorIndicies[choice]);
    }

    //Remove later
    //Mirrors count as enemies. When they are hit, instead of doing usual enemy stuff,
    //they call this method.
    public void isHit(float damage, float staggerDamage)
    {
        if (canBeHarmed)
        {
            currentHealth -= damage;
            //damageFlash.FlashStart();
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            healthPercent = currentHealth / maxHealth;
            //enemyHealthBar.setHealth(healthPercent);

            //Proceed to phase 1.5 at 2/3 health
            if(healthPercent <= 0.66 && phase == 1)
            {
                Debug.Log("Proceeding to phase 2");
                //Debug.Log("Boss health: " + currentHealth);
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

    public void phase2CompletionCheck(){
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        //Debug.Log(enemies.Length);
        if(enemies.Length < 5){ //since there are 4 mirrors it will check if all enemies but the mirrors are dead
            canBeHarmed = true;
            phase += 1;
            Debug.Log("Phase 2 complete");
            btRunner.tree.rootNode.Abort();
        }
    }
    
    public IEnumerator projectileAttack() {
        Projectile projectile = currPosessedMirror.projectilePrefab;
        float secondsPerProjectile = 1.0f / projectilesPerSec; // Calculate the time between each projectile
        float elapsedTime = 0.0f; // Tracks time since the attack started
        int patternChoice = (UnityEngine.Random.Range(0, 2)); //chooses random pattern
        while (elapsedTime < projectileAttackDuration) {
            setAllMirrorAnimations("Shooting", true);
            foreach (MirrorBossMirror mirror in mirrors) {
                Vector3 spawnPosition = mirror.transform.position;
                spawnPosition.y -= 0.5f; //sets spawn position slightly lower than center of mirror
                // Instantiate a clone of the projectile prefab at the mirror's position and rotation
                Projectile projectileClone = Instantiate(projectile, spawnPosition, mirror.transform.rotation);
                float angleStep = projectilePattern(patternChoice, elapsedTime); //gets the angle to shoot depending on pattern
                Vector3 stepVector = Quaternion.AngleAxis(angleStep, Vector3.up) * mirror.transform.right; //calculates the angle to shoot

                // Randomize the projectile lifetime within the range of current value +- 1
                float randomizedLifetime = mirror.projectileLifetime + UnityEngine.Random.Range(-1.5f, 2.5f);

                // Initialize and Activate the clone
                projectileClone.Initialize(mirror.projectileSpeed, randomizedLifetime, mirror.projectileDamage, 1f, stepVector, mirror.trashSpawnChance);
                projectileClone.gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(secondsPerProjectile); // Waits until shooting next projectile
            elapsedTime += secondsPerProjectile; // Increment the elapsed time
        }
        isCoroutineRunning = false;
        setAllMirrorAnimations("Shooting", false);
    }

    public void setAllMirrorAnimations(string animName, bool setTo){
        foreach (MirrorBossMirror mirror in mirrors) {
            Animator anim = mirror.GetComponentInChildren<Animator>();
            anim.SetBool(animName, setTo);
        }
    }

    public float projectilePattern(int patternNum, float projectileCounter){
        if(patternNum == 0){ //shoots in a cos wave pattern
            return (float)Math.Cos(projectileCounter) * projectileMaxAngle;
        }
        else if(patternNum == 1){ //shoots randomly
            return UnityEngine.Random.Range(-projectileMaxAngle, projectileMaxAngle);
        }
        return 0.0f;
    }

    public void spawnEnemies(){
        //similar to how the game manager spawns enemies
        Vector3 playerPos = player.transform.position;
        int numberOfEnemies = 2;
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Bounds spawnBounds = spawnArea.GetComponent<MeshCollider>().bounds;
            Vector3 position;
            do
            {
                position = new Vector3(
                    UnityEngine.Random.Range(spawnBounds.min.x, spawnBounds.max.x),
                    spawnArea.transform.position.y,
                    UnityEngine.Random.Range(spawnBounds.min.z, spawnBounds.max.z)
                );
            } while (Vector3.Distance(playerPos, position) < 3);
            GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        }
        enemyPrefab.SetActive(false);
    }
}
