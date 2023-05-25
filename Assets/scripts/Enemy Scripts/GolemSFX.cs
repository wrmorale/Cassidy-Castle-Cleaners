using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemSFX : MonoBehaviour
{
    public AudioSource animationSoundPlayer;
    public AudioClip stepSfx;
    public float stepVolume;
    public AudioClip light1Sfx;
    public float light1Volume;
    public AudioClip light2Sfx;
    public float light2Volume;
    public AudioClip spinSfx;
    public float spinVolume;
    public AudioClip dashSfx;
    public float dashVolume;
    public AudioClip deathSfx;
    public float deathVolume;

    private void playstepSfx(){
        animationSoundPlayer.PlayOneShot(stepSfx, stepVolume);
    }

    private void playlight1Sfx(){
        animationSoundPlayer.PlayOneShot(light1Sfx, light1Volume);
    }

    private void playlight2Sfx(){
        animationSoundPlayer.PlayOneShot(light2Sfx, light2Volume);
    }

    private void playspinSfx(){
        animationSoundPlayer.PlayOneShot(spinSfx, spinVolume);
    }

    private void playdashSfx(){
        animationSoundPlayer.PlayOneShot(dashSfx, dashVolume);
    }

    private void playdeathSfx(){
        animationSoundPlayer.PlayOneShot(deathSfx, deathVolume);
    }
}
