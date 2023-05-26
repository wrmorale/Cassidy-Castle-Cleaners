using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorBossSoundManager : MonoBehaviour
{
    public AudioSource animationSoundPlayer;
    public AudioClip teleportSfx;
    public float teleportVolume;
    public AudioClip isHitSfx;
    public float isHitVolume;
    public AudioClip slamSfx;
    public float slamVolume;
    public AudioClip deathSfx;
    public float deathVolume;
    public AudioClip laughSfx;
    public float laughVolume;

    public void playTeleportsfx(){
        animationSoundPlayer.PlayOneShot(teleportSfx, teleportVolume);
    }

    public void playIsHitsfx(){
        animationSoundPlayer.PlayOneShot(isHitSfx, isHitVolume);
    }

    public void playSlamsfx(){
        animationSoundPlayer.PlayOneShot(slamSfx, slamVolume);
    }

    public void playDeathsfx(){
        animationSoundPlayer.PlayOneShot(deathSfx, deathVolume);
    }

    public void playLaughsfx(){
        animationSoundPlayer.PlayOneShot(laughSfx, laughVolume);
    }

}
