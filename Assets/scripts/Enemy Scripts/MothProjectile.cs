using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothProjectile : Projectile
{
    public GameObject dustPilePrefab;

    protected override void Update()
    {
        base.Update();
        transform.position += heading * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            player.isHit(damage);
            Destroy(gameObject);
        }
        //Can change to other tags like "Room" and such 
        else
        {
            Debug.Log("Projectile hit something");
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (UnityEngine.Random.value <= trashSpawnChance)
        {
            float groundOffset = 0.4f; // Adjust this value to control the height offset from the ground
            Vector3 groundPosition = new Vector3(transform.position.x, groundOffset, transform.position.z);
            if(dustPilePrefab != null){
                this.lifetime = 3;
                GameObject dustPile = Instantiate(dustPilePrefab, groundPosition, Quaternion.identity);
                dustPile.SetActive(true);
            }
            // Customize the dust pile prefab as needed
        }
    }
}
