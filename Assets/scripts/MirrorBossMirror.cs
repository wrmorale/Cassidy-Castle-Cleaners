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

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
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

    public override void isHit(float damage)
    {
        if (entityPossessing)
        {
            mainScript.isHit(damage);
        }
        else
        {
            Debug.LogError("This mirror is not posessed!");
        }
    }

}
