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
    public float audioLevel;

    void Start()
    {
        animationSoundPlayer = GetComponent<AudioSource>();
    }

    private void playHopsfx(){
        animationSoundPlayer.PlayOneShot(hopsfx, audioLevel * 0.4f);
    }

    private void playChargesfx(){
        animationSoundPlayer.PlayOneShot(chargesfx, audioLevel);
    }

    private void playPouncesfx(){
        animationSoundPlayer.PlayOneShot(pouncesfx, audioLevel);
    }
}
