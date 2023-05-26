using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustBunnySFX : MonoBehaviour
{
    [Header("Audio Files")]
    AudioSource animationSoundPlayer;
    public AudioClip hopsfx;
    public float hopLevel;
    public AudioClip chargesfx;
    public float chargeLevel;
    public AudioClip pouncesfx;
    public float pounceLevel;

    void Start()
    {
        animationSoundPlayer = GetComponent<AudioSource>();
    }

    private void playHopsfx(){
        animationSoundPlayer.PlayOneShot(hopsfx, hopLevel);
    }

    private void playChargesfx(){
        animationSoundPlayer.PlayOneShot(chargesfx, chargeLevel);
    }

    private void playPouncesfx(){
        animationSoundPlayer.PlayOneShot(pouncesfx, pounceLevel);
    }
}
