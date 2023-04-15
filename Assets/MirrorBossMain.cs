using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorBossMain : MonoBehaviour //Will derive from Enemy class later
{
    //When changing mirrors, will need to change the body collider to be the corresponding mirror

    public List<MirrorBossMirror> mirrors;
    [HideInInspector] public MirrorBossMirror currPosessedMirror;
    int currMirrorIndex;
    bool canBeHarmed = false; //Starts immune to damage during the starting cutscene, though I suppose we could just make it that none of the mirrors are posessed at first.

    // Start is called before the first frame update
    void Start()
    {
        currPosessedMirror = mirrors[0];
        currMirrorIndex = 0;
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
        currPosessedMirror = mirrors[mirrorIndex];
        currMirrorIndex = mirrorIndex;
        Debug.Log("Posessed mirror " + mirrorIndex);
        //Then for the mirror that just became active, we play an animation for the entity appearing.
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

        int choice = (Random.Range(0, mirrorIndicies.Count));
        PosessMirror(mirrorIndicies[choice]);
    }
}
