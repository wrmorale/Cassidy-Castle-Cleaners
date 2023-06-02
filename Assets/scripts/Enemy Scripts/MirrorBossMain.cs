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
    [SerializeField] int aimOnCycle = 3; //Which projectile cycle to shoot at the player on

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
        currPosessedMirror.mirrorAudioManager.playTeleportsfx();
        currMirrorIndex = mirrorIndex;

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
            currPosessedMirror.mirrorAudioManager.playIsHitsfx();
            //damageFlash.FlashStart();
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            healthPercent = currentHealth / maxHealth;
            bossHealthBar.setHealth(healthPercent);

            //Proceed to phase 1.5 at 1/3 health
            if(healthPercent <= 0.33 && phase == 1)
            {
                btRunner.tree.rootNode.Abort();
                canBeHarmed = false;
                phase += 1;
                setShootingWarning(false); //Remove later
            }

            if (currentHealth <= 0)
            {
                // Destroy the cube when it has no health left
                //this should work for death animation but not all enemies have one so it gets errors
                //animator.SetBool("Death", true);
                currPosessedMirror.mirrorAudioManager.playDeathsfx();
                //StartCoroutine(waitForAnimation("Death"));

                if (levelLoader) //Load the ending scene
                    levelLoader.LoadTargetLevel("Win_scene");
                else
                    Debug.LogError("[MirrorBossMain] Where's the level loader??");

                //Destroy(gameObject);
                StartCoroutine(DelayedDestroy());
            }
        }
        else
        {
            Debug.LogWarning("Boss cannot be hurt right now!");
        }
    }

    // Coroutine for delayed destruction
    IEnumerator DelayedDestroy(){
        yield return new WaitForSeconds(1f); // Wait for 1 second
        Destroy(gameObject); // Destroy the object after the delay
    }

    public void phase2CompletionCheck(){
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        if(enemies.Length < 5 && finishedSpawning){ //since there are 4 mirrors it will check if all enemies but the mirrors are dead
            canBeHarmed = true;
            phase += 1;
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

                float angleStep = 0;
                int offset = 0;
                if(phase == 1)
                {
                    if ((cycle + offset) % aimOnCycle == 0)
                    {
                        //Shoot projectile directly at the player's xz position
                        angleStep = getAngleToPlayer(i);
                    }
                    else
                    {   //Follow the usual projectile pattern
                        angleStep = projectilePattern(patternChoice, elapsedTime); // Gets the angle to shoot depending on the pattern

                    }
                }
                else //In phase 2, alternate shooting the tracking ones from the new and old mirrors
                {
                    if (i > 3)
                        offset = aimOnCycle / 2;
                    if ((cycle + offset) % aimOnCycle == 0)
                    {
                        //Shoot projectile directly at the player's xz position
                        angleStep = getAngleToPlayer(i);
                    }
                    else
                    {   //Follow the usual projectile pattern
                        angleStep = projectilePattern(patternChoice, elapsedTime); // Gets the angle to shoot depending on the pattern

                    }
                }
                Vector3 stepVector = Quaternion.AngleAxis(angleStep, Vector3.up) * mirrors[i].transform.right; // Calculates the angle to shoot

                // Randomize the projectile lifetime within the range of the current value +- 1
                //float randomizedLifetime = mirrors[i].projectileLifetime + UnityEngine.Random.Range(-1.5f, 2.0f);

                // Initialize and activate the clone
                float shardPileSpawnChance = mirrors[i].trashSpawnChance;
                if(phase > 1){
                    shardPileSpawnChance = shardPileSpawnChance/2;
                }
                projectileClone.Initialize(mirrors[i].projectileSpeed, 5.0f, mirrors[i].projectileDamage, 1f, stepVector, shardPileSpawnChance);
                if ((cycle + offset) % 3 == 0)
                {
                    //projectileClone.transform.localScale = new Vector3(1, 5, 1); //Used to see which projectiles are shot directly at player
                }
                projectileClone.gameObject.SetActive(true);

                // Rotate the projectile to face its direction
                projectileClone.transform.rotation = Quaternion.LookRotation(stepVector);

                //Play sound
                if(i < 4) //Avoid getting too loud...
                    mirrors[i].mirrorAudioManager.playShootShardSfx();
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

    //Returns an angle from the mirror's x-axis to the player's position (used for projectile aiming)
    float getAngleToPlayer(int mirrorIndex)
    {
        Vector3 toPlayer = player.position - mirrors[mirrorIndex].transform.position;
        toPlayer.y = 0;
        Quaternion newRotation = Quaternion.LookRotation(toPlayer, mirrors[mirrorIndex].transform.up);
        //Z-axis is forward
        //Mirror 2: toPlayer angle is like 340 degrees. But the rotation of the actual mirror is only -90 (which I guess could also be considered 270 degrees)
        //270 - 340 would be -70 degrees, the right amount for the projectile coming from mirror 2.

        //The z and x of the toPlayer angle...
        //And the z and x of the mirror's forward...(Note: mirror's forward is actually its side)
        Vector2 toPlayerZX = new Vector2(toPlayer.z, toPlayer.x);
        Vector3 mirrorForward = mirrors[mirrorIndex].transform.forward;
        Vector2 mirrorForwardZX = new Vector2(mirrorForward.z, mirrorForward.x);

        
        float toPlayerAngle = Vector2.SignedAngle(mirrorForwardZX, toPlayerZX) - 90;

        //Constrain to be within maxAngle?
        return Mathf.Clamp(toPlayerAngle, -projectileMaxAngle, projectileMaxAngle); ;
    }

    public void spawnEnemies(){
        //similar to how the game manager spawns enemies
        Vector3 playerPos = player.transform.position;
        GameObject[] enemies = new GameObject[numberOfEnemies];
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
            enemy.GetComponent<Enemy>().startAggro = true;
            Debug.Log("Spawned enemy");
        }
        enemyPrefab.SetActive(false);
        finishedSpawning = true;

        foreach(MirrorBossMirror mirror in mirrors)
        {
            mirror.mirrorAudioManager.playSpawnEnemySfx();
        }
    }

    //Can be removed after shooting animation is added
    public void setShootingWarning(bool setTo)
    {
        foreach (MirrorBossMirror mirror in mirrors)
        {
            if(setTo == true)
            {
                mirror.glassShardPortal.Play();
            }
            else
            {
                mirror.glassShardPortal.Stop();
            }
        }
    }
}
