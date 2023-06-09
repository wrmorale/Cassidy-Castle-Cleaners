using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MirrorBossMirror : Enemy
{
    [Header("Mirror Variables - Ignore everything else")]
    public bool entityPossessing = false;
    public GameObject faceRender;
    MirrorBossMain mainScript;
    public GameObject tempProjectileWarning;
    public ParticleSystem glassShardPortal;

    [Header("Projectile")]
    [SerializeField] public ShardProjectile projectilePrefab;
    public Transform bulletSpawn;

    [Header("Shockwave")]
    public MirrorSlamShockwave shockwave;
    [SerializeField] float shockwaveDamage = 4.0f;
    [SerializeField] float shockwaveStartScale = 0.75f;
    [SerializeField] float shockwaveEndScale = 4.0f;
    [SerializeField] float shockwaveDuration = 0.5f;
    [SerializeField] Transform shockwaveSpawnPoint;

    [Header("Other")]
    [SerializeField] public Renderer mirrorRenderer;
    public MirrorBossSoundManager mirrorAudioManager;

    // Start is called before the first frame update
    void Start()
    {
        SetPossessed(entityPossessing);
        mainScript = GetComponentInParent<MirrorBossMain>();
    }

    public void SetPossessed(bool makePosessed) {
        if (makePosessed)
        {
            entityPossessing = true;
            faceRender.SetActive(true);
            tag = "Enemy";
        }
        else
        {
            entityPossessing = false;
            faceRender.SetActive(false);
            tag = "Untagged"; //Prevents lock-on targeting
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void isHit(float damage, float staggerDamage)
    {
        if (entityPossessing)
        {
            mainScript.isHit(damage, staggerDamage);
        }
        else
        {
            Debug.LogWarning("This mirror is not posessed!");
        }
    }
    
    public void spawnShockwave()
    {
        if(shockwaveSpawnPoint && shockwave)
        {
            MirrorSlamShockwave shockwaveInstance = Instantiate(shockwave, shockwaveSpawnPoint.transform);
            shockwaveInstance.Initialize(shockwaveStartScale, shockwaveEndScale, shockwaveDuration);
            shockwaveInstance.gameObject.GetComponent<collisionDetection>().damage = shockwaveDamage;
            shockwave.gameObject.SetActive(true);
            shockwaveSpawnPoint.gameObject.GetComponent<ParticleSystem>().Play();
        }
        else
        {
            Debug.LogError("One of the shock wave prefabs is missing.");
        }
    }
}
