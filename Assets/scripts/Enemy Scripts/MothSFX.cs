using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothSFX : MonoBehaviour
{
    AudioSource animationSoundPlayer;
    public AudioClip attackWindupsfx;
    public float attackWindupLevel;
    public AudioClip attacksfx;
    public float attackLevel;

    void Start()
    {
        animationSoundPlayer = GetComponent<AudioSource>();
    }

    private void playWindUpsfx(){
        animationSoundPlayer.PlayOneShot(attackWindupsfx, attackWindupLevel);
    }

    private void playAttacksfx(){
        animationSoundPlayer.PlayOneShot(attacksfx, attackLevel);
    }
}
