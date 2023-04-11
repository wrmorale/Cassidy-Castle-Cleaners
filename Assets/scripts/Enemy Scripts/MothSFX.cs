using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothSFX : MonoBehaviour
{
    [Header("Audio Files")]
    AudioSource animationSoundPlayer;
    public AudioClip attackWindupsfx;
    public AudioClip attacksfx;

    void Start()
    {
        animationSoundPlayer = GetComponent<AudioSource>();
    }

    private void playWindUpsfx(){
        animationSoundPlayer.PlayOneShot(attackWindupsfx, 0.8F);
    }

    private void playAttacksfx(){
        animationSoundPlayer.PlayOneShot(attacksfx, 0.8F);
    }
}
