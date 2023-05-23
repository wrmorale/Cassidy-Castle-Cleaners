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

    [Header("Projectile Stats")]
    [SerializeField] public Projectile projectilePrefab;
    [SerializeField] public float projectileSpeed;
    [SerializeField] public float projectileLifetime;
    [SerializeField] public float projectileDamage;
    [SerializeField] public float trashSpawnChance;
    public Transform bulletSpawn;

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
        }
        else
        {
            entityPossessing = false;
            faceRender.SetActive(false);
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
    
}
