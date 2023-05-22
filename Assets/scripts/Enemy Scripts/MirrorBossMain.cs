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
    [HideInInspector] public float currentHealth;
    private float healthPercent = 1.0f;
    private EnemyHealthBar bossHealthBar;

    [Header("Projectile Stats")]
    [SerializeField] float projectileAttackDuration = 5.0f;
    [SerializeField] float projectilesPerSec = 3.0f;
    [SerializeField] float projectileMaxAngle = 25.0f;

    [Header("Enemy Spawns")]
    [SerializeField] public int numberOfEnemies;
    [SerializeField] GameObject spawnArea;
    public GameObject enemyPrefab;
    private bool finishedSpawning = false;

    /*I should probably experiment a bit with the golem before I give the others their tasks...*/
    [Header("Aggro Status")]
    public bool aggro = false;
    public Transform player;
    public int phase; //1 = phase 1, 2 = spawn enemies, 3 = final phase, 4+ = defeated

    [HideInInspector] public bool isCoroutineRunning = false;

    [Header("Other")]
    public LevelLoader levelLoader;

    // Start is called before the first frame update
    void Start()
    {
        currPosessedMirror = mirrors[0];
        currMirrorIndex = 0;
        btRunner = GetComponentInChildren<BehaviourTreeRunner>();

        //Remove later
        canBeHarmed = true;
        currentHealth = maxHealth;

        bossHealthBar = GetComponentInChildren<EnemyHealthBar>();
        bossHealthBar.setMaxHealth(healthPercent);
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
            bossHealthBar.setHealth(healthPercent);

            //Proceed to phase 1.5 at 1/3 health
            if(healthPercent <= 0.33 && phase == 1)
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

                if (levelLoader) //Load the ending scene
                    levelLoader.LoadTargetLevel("Win_scene");
                else
                    Debug.LogError("[MirrorBossMain] Where's the level loader??");

                Destroy(gameObject);
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
        if(enemies.Length < 5 && finishedSpawning){ //since there are 4 mirrors it will check if all enemies but the mirrors are dead
            canBeHarmed = true;
            phase += 1;
            Debug.Log("Phase 2 complete");
            btRunner.tree.rootNode.Abort();
        }
    }
    
    public IEnumerator projectileAttack()
    {
        Projectile projectile = currPosessedMirror.projectilePrefab;
        float secondsPerProjectile = 1.0f / projectilesPerSec; // Calculate the time between each projectile
        float elapsedTime = 0.0f; // Tracks time since the attack started
        int patternChoice = 1; // UnityEngine.Random.Range(0, 2); // Chooses random pattern
        int cycle = 1;

        /*
         Santi's notes
         So it chooses a pattern once at the start of the attack cycle...
         Every mirror shoots a projectile at the same time, then waits before all shooting the next one

        My pattern might not work well because the two unaimed-shots will not be spaced equally.
         */

        while (elapsedTime < projectileAttackDuration)
        {
            setAllMirrorAnimations("Shooting", true);
            for(int i = 0; i < mirrors.Count; i++)
            {
                /*Could add a check here for the main mirror to shoot directly at the player
                 Wouldn't even be necessary if all mirrors EXCEPT the main one were always shooting*/

                Vector3 spawnPosition = mirrors[i].transform.position;

                // Instantiate a clone of the projectile prefab at the mirror's position and rotation
                Projectile projectileClone = Instantiate(projectile, spawnPosition, mirrors[i].transform.rotation);

                Vector3 stepVector;
                int offset = 0;
                if(phase < 2) //Phase 1
                {   //Each pair of parallel mirrors
                    if(i > 1)
                        offset = 3;
                }
                else
                {
                    if (i > 3) //The newly spawned mirrors
                        offset = 3;
                }
                if((cycle + offset) % 6 == 0)
                {
                    //Shoot projectile directly at the player's xz position
                    Vector3 toPlayer = player.position - mirrors[i].transform.position;
                    toPlayer.y = 0;
                    stepVector = toPlayer.normalized;
                    //Constrain to be within maxAngle?
                }
                else
                {   //Follow the usual projectile pattern
                    float angleStep = projectilePattern(patternChoice, elapsedTime); // Gets the angle to shoot depending on the pattern
                    stepVector = Quaternion.AngleAxis(angleStep, Vector3.up) * mirrors[i].transform.right; // Calculates the angle to shoot
                }

                // Randomize the projectile lifetime within the range of the current value +- 1
                float randomizedLifetime = mirrors[i].projectileLifetime + UnityEngine.Random.Range(-1.5f, 2.0f);

                // Initialize and activate the clone
                projectileClone.Initialize(mirrors[i].projectileSpeed, 5.0f, mirrors[i].projectileDamage, 1f, stepVector, mirrors[i].trashSpawnChance);
                if ((cycle + offset) % 6 == 0)
                {
                    //projectileClone.transform.localScale = new Vector3(1, 5, 1); //Used to see which projectiles are shot directly at player
                }
                projectileClone.gameObject.SetActive(true);

                // Rotate the projectile to face its direction
                projectileClone.transform.rotation = Quaternion.LookRotation(stepVector);
            }
            yield return new WaitForSeconds(secondsPerProjectile); // Waits until shooting the next projectile
            elapsedTime += secondsPerProjectile; // Increment the elapsed time
            cycle += 1;
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
        /*Returns a number ranging from projectileMaxAngle to -projectileMaxAngle*/
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
            enemy.SetActive(true);
        }
        enemyPrefab.SetActive(false);
        finishedSpawning = true;
    }
}
