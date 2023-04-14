using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorBossMirror : MonoBehaviour
{
    public bool entityPossessing = false;
    public Material matInactive;
    public Material matPosessed;
    MeshRenderer mesh;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        SetPossessed(entityPossessing);
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
}
