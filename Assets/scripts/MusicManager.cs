using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip battleMusic;
    public AudioClip menuMusic;
    public AudioClip calmMusic;

    // Start is called before the first frame update
    void Start()
    {
        //audioSource = GetComponent<AudioSource>();
    }

    public void playBattleMusic()
    {
        audioSource.PlayOneShot(battleMusic, 1.0F);
    }

    public void playMenuMusic()
    {
        audioSource.PlayOneShot(menuMusic, 3.0F);
    }

    public void playCalmMusic()
    {
        audioSource.PlayOneShot(calmMusic, 1.0F);
    }
    
    public void StopMusic()
    {
        audioSource.Stop();
    }
}
