using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    public void TriggerCutscene ()
    {
        FindObjectOfType<CutsceneManager>().StartCutscene();
    }
}
