using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorBossMain : MonoBehaviour //Will derive from Enemy class later
{
    //When changing mirrors, will need to change the body collider to be the corresponding mirror

    public List<MirrorBossMirror> mirrors;
    [HideInInspector] public MirrorBossMirror currPosessedMirror;

    // Start is called before the first frame update
    void Start()
    {
        currPosessedMirror = mirrors[0];
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
        //Then for the mirror that just became active, we play an animation for the entity appearing.
    }
}
