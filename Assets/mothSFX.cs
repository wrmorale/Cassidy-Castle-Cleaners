using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mothSFX : MonoBehaviour
{
    [Header("Audio Files")]
    AudioSource animationSoundPlayer;
    public AudioClip attackWindupsfx;
    public AudioClip attacksfx;

    // Start is called before the first frame update
    void Start()
    {
        animationSoundPlayer = GetComponent<AudioSource>();
    }

    private void playWindUpsfx(){
        animationSoundPlayer.PlayOneShot(attackWindupsfx, 2.0F);
    }

    private void playAttacksfx(){
        animationSoundPlayer.PlayOneShot(attacksfx, 2.0F);
    }
}
