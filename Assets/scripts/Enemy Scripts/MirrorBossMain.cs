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

    [Header("Textures")]
    [SerializeField] private Texture texture75Percent;
    [SerializeField] private Texture texture50Percent;
    [SerializeField] private Texture texture25Percent;

    [Header("Stats")]
    [SerializeField] public string enemyName;
    [SerializeField] public float maxHealth = 1.0f; //Shouldn't these be integers?
    [HideInInspector] public float currentHealth;
    private float healthPercent = 1.0f;
    private EnemyHealthBar bossHealthBar;

    [Header("Projectile Stats")]
    [SerializeField] float projectileAttackDuration = 5.0f;
    [SerializeField] float volleysPerSec = 3.0f;
    [SerializeField] float projectileMaxAngle = 25.0f;
    public int projectilesPerVolley = 10;
    [SerializeField] public float projectileSpeed = 4;
    [SerializeField] public float projectileLifetime = 2;
    [SerializeField] public float projectileDamage = 3;
    [SerializeField] public float trashSpawnChance = 0.055f;
    //Trash will only spawn if current dust piles < max dust piles
    GameManager gm;

    [Header("Enemy Spawns")]
    [SerializeField] GameObject spawnArea;
    public GameObject bunnyPrefab;
    public int bunnyAmount;
    public GameObject mothPrefab;
    public int mothAmount;
    public GameObject golemPrefab;
    public int golemAmount;
    private bool finishedSpawning = false;

    /*I should probably experiment a bit with the golem before I give the others their tasks...*/
    [Header("Aggro Status")]
    public bool aggro = false;
    public Transform player;
    public int phase; //1 = phase 1, 2 = spawn enemies, 3 = final phase, 4 = dead
    public bool setupPhase3 = false;

    [HideInInspector] public bool isCoroutineRunning = false;

    [Header("Other")]
    public LevelLoader levelLoader;
    public GameObject centerObject;


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

        // Get the renderer component
        //bossRenderer = GetComponent<Renderer>();
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(phase == 2){
            phase2CompletionCheck();

            // Move the center object down in the global y direction
            if(centerObject != null){
                float descentSpeed = 0.8f; // Adjust the descent speed as needed
                centerObject.transform.position -= new Vector3(0f, descentSpeed * Time.deltaTime, 0f);
            }
            // Destroy the center object when it reaches a certain height
            float destroyHeight = -2f; // Adjust the destroy height as needed
            if (centerObject.transform.position.y <= destroyHeight)
            {
                Destroy(centerObject);
            }
            
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
        //currPosessedMirror.mirrorAudioManager.playTeleportsfx();
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
            if(damage > 0)
                currPosessedMirror.mirrorAudioManager.playIsHitsfx();
            //damageFlash.FlashStart();
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            healthPercent = currentHealth / maxHealth;
            bossHealthBar.setHealth(healthPercent);

            //Proceed to phase 1.5 at 1/3 health
            if(healthPercent <= 0.33 && phase == 1)
            {
                abortBT();
                canBeHarmed = false;
                phase += 1;
                setShootingWarning(false); //Remove later
            }

            if (currentHealth <= 0) //Dead now
            {
                canBeHarmed = false; //Prevent from taking further damage now that it's dead
                phase = 4;
                currPosessedMirror.mirrorAudioManager.playDeathsfx();
                abortBT();
            }
        }
        else
        {
            Debug.LogWarning("Boss cannot be hurt right now!");
        }

        //changes texture based on health
        if (healthPercent <= 0.75f && healthPercent > 0.5f)
        {
            changeMirrorTextures(texture75Percent);
        }
        else if (healthPercent <= 0.5f && healthPercent > 0.25f)
        {
            changeMirrorTextures(texture50Percent);
        }
        else if (healthPercent <= 0.25f)
        {
            changeMirrorTextures(texture25Percent);
        }
    }

    private void changeMirrorTextures(Texture texture)
    {
        foreach (MirrorBossMirror mirror in mirrors){
            mirror.mirrorRenderer.material.mainTexture = texture;
        }
    }

    public void loadEndingScene()
    {
        if (levelLoader) //Load the ending scene
            levelLoader.LoadTargetLevel("Win_scene");
        else
            Debug.LogError("[MirrorBossMain] Where's the level loader??");
        Destroy(gameObject, 1f); // Destroy the object after the delay
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
        ShardProjectile projectile = currPosessedMirror.projectilePrefab;
        float secondsPerProjectile = 1.0f / volleysPerSec; // Calculate the time between each projectile
        float elapsedTime = 0.0f; // Tracks time since the attack started

        while (elapsedTime < projectileAttackDuration)
        {
            for (int i = 0; i < mirrors.Count; i++)
            {
                /*Could add a check here for the main mirror to shoot directly at the player
                 Wouldn't even be necessary if all mirrors EXCEPT the main one were always shooting*/

                Vector3 spawnPosition = mirrors[i].bulletSpawn.position;

                // Instantiate a clone of the projectile prefab at the mirror's position and rotation
                Projectile projectileClone = Instantiate(projectile, spawnPosition, mirrors[i].transform.rotation);

                float angleStep = 0;
                angleStep = getAngleToPlayer(i);
                Vector3 stepVector = Quaternion.AngleAxis(angleStep, Vector3.up) * mirrors[i].transform.right; // Calculates the angle to shoot

                // Initialize and activate the clone
                float shardPileSpawnChance = trashSpawnChance;
                if (phase > 1)
                {
                    shardPileSpawnChance = shardPileSpawnChance / 2;
                }
                projectileClone.Initialize(projectileSpeed, 5.0f, projectileDamage, 1f, stepVector, shardPileSpawnChance);
                
                projectileClone.gameObject.SetActive(true);

                // Rotate the projectile to face its direction
                projectileClone.transform.rotation = Quaternion.LookRotation(stepVector);

                //Play sound
                if (i < 4) //Avoid getting too loud...
                    mirrors[i].mirrorAudioManager.playShootShardSfx();
            }

            yield return new WaitForSeconds(secondsPerProjectile); // Waits until shooting the next volley
            elapsedTime += secondsPerProjectile; // Increment the elapsed time
        }
        isCoroutineRunning = false;
    }

    public float projectilePattern(int patternNum, float projectileCounter){
        /*Returns a number ranging from projectileMaxAngle to -projectileMaxAngle
         The amount each time is based on the flat elapsed time. Essentially put in
        secondsPerProjectile and that's the amount it will change*/

        /*New desired pattern: Fire a massive spread of projectiles 
         Start at +/-proejctileMaxAngle and move by 
         */

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

    public void spawnEnemies()
    {
        Vector3 playerPos = player.transform.position;

        // Spawn bunny enemies
        for (int i = 0; i < bunnyAmount; i++)
        {
            SpawnEnemy(bunnyPrefab, playerPos);
        }

        // Spawn moth enemies
        for (int i = 0; i < mothAmount; i++)
        {
            SpawnEnemy(mothPrefab, playerPos);
        }

        // Spawn golem enemies
        for (int i = 0; i < golemAmount; i++)
        {
            SpawnEnemy(golemPrefab, playerPos);
        }

        // Deactivate enemyPrefab
        //enemyPrefab.SetActive(false);

        finishedSpawning = true;

        foreach (MirrorBossMirror mirror in mirrors)
        {
            mirror.mirrorAudioManager.playSpawnEnemySfx();
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab, Vector3 playerPos)
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
        //Debug.Log("Spawned enemy");
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

    public void abortBT()
    {
        btRunner.tree.rootNode.Abort();
        StopAllCoroutines();
    }

}
