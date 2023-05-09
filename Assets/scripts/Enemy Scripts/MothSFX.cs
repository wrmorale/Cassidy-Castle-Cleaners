using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothSFX : MonoBehaviour
{
    AudioSource animationSoundPlayer;
    public AudioClip attackWindupsfx;
    public AudioClip attacksfx;
    public float audioLevel;

    void Start()
    {
        animationSoundPlayer = GetComponent<AudioSource>();
    }

    private void playWindUpsfx(){
        animationSoundPlayer.PlayOneShot(attackWindupsfx, audioLevel);
    }

    private void playAttacksfx(){
        animationSoundPlayer.PlayOneShot(attacksfx, audioLevel);
    }
}
