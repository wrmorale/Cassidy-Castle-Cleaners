using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorBossMirror : Enemy
{
    [Header("Mirror Variables - Ignore everything else")]
    public bool entityPossessing = false;
    public Material matInactive;
    public Material matPosessed;
    MeshRenderer mesh;
    MirrorBossMain mainScript;

    [Header("Projectile Stats")]
    [SerializeField] public Projectile projectilePrefab;
    [SerializeField] public float projectileSpeed;
    [SerializeField] public float projectileLifetime;
    [SerializeField] public float projectileDamage;
    public Transform bulletSpawn;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponentInChildren<MeshRenderer>();
        SetPossessed(entityPossessing);
        mainScript = GetComponentInParent<MirrorBossMain>();
    }

    public void SetPossessed(bool makePosessed) {
        if (makePosessed)
        {
            entityPossessing = true;
            mesh.material = matPosessed;
        }
        else
        {
            entityPossessing = false;
            mesh.material = matInactive;
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
            mainScript.isHit(damage);
        }
        else
        {
            Debug.LogWarning("This mirror is not posessed!");
        }
    }
    
}
