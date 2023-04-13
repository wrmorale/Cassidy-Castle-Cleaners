using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemSFX : MonoBehaviour
{
    [Header("Audio Files")]
    AudioSource animationSoundPlayer;
    public AudioClip footstepsfx;
    public AudioClip chargesfx;

    void Start()
    {
        animationSoundPlayer = GetComponent<AudioSource>();
    }

    private void playStepsfx(){
        animationSoundPlayer.PlayOneShot(footstepsfx, .5F);
    }

    private void playChargesfx(){
        animationSoundPlayer.PlayOneShot(chargesfx, 1.2F);
    }

}
