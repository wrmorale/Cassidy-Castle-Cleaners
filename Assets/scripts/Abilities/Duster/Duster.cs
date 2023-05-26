using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duster : Projectile
{
    protected override void Update()
    {
        base.Update();
        transform.position += heading * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy") 
        {
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.isHit(damage, stagger);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        
    }
}
