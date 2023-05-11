using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private float damage;
    private float stagger;
    [SerializeField] private float lifetime = 0.8f;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float audioLevel;
    
    private GameObject playerObj;
    private AudioSource audioSource;

    public void Initialize(float damage, float stagger)
    {
        //Audio Stuff
        playerObj = GameObject.Find("Player");
        audioSource = playerObj.GetComponentInChildren<AudioSource>();

        this.damage = damage;
        this.stagger = stagger;
        audioSource.PlayOneShot(audioClip, audioLevel);
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy") 
        {
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.isHit(damage, stagger);
        }
        if (other.tag == "DustPile")
        {
            DustPile dustPile = other.GetComponent<DustPile>();
            dustPile.isHit(damage);
        }
        if (other.tag == "Furniture")
        {
            Furniture furniture = other.GetComponent<Furniture>();
            furniture.isHit(damage);
        }
    }
}
