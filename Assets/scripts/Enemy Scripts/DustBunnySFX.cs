using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustBunnySFX : MonoBehaviour
{
    [Header("Audio Files")]
    AudioSource animationSoundPlayer;
    public AudioClip hopsfx;
    public AudioClip chargesfx;
    public AudioClip pouncesfx;

    void Start()
    {
        animationSoundPlayer = GetComponent<AudioSource>();
    }

    private void playHopsfx(){
        animationSoundPlayer.PlayOneShot(hopsfx, .5F);
    }

    private void playChargesfx(){
        animationSoundPlayer.PlayOneShot(chargesfx, 1.2F);
    }

    private void playPouncesfx(){
        animationSoundPlayer.PlayOneShot(pouncesfx, 1.5F);
    }
}
