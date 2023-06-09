using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mop : MonoBehaviour
{
    private float radius = 5f;
    private float damage = 1f;
    private float stagger = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.isHit(damage, stagger);
            }
        }
        else if (other.CompareTag("DustPile"))
        {
            DustPile dustPile = other.GetComponent<DustPile>();
            if (dustPile != null)
            {
                dustPile.isHit(damage);
            }
        }
    }

    public void Initialize(float damage, float stagger, float radius)
    {
        this.damage = damage;
        this.stagger = stagger;
        this.radius = radius;
    }
}
