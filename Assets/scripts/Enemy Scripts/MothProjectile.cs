using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothProjectile : Projectile
{
    private GameObject spawnPlane; // Reference to the spawn plane object
    public GameObject dustPilePrefab;

    private void Start(){
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("spawnPlane");
        spawnPlane = taggedObjects[0];
    }

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
        else if(other.tag == "Furniture"){
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (UnityEngine.Random.value <= trashSpawnChance)
        {
            float groundOffset = 0.3f; // Adjust this value to control the height offset from the ground
            Vector3 groundPosition = new Vector3(transform.position.x, groundOffset, transform.position.z);
            if (IsWithinBounds(groundPosition) && dustPilePrefab != null)
            {
                GameObject dustPile = Instantiate(dustPilePrefab, groundPosition, Quaternion.identity);
                dustPile.SetActive(true);
            }
        }
    }
    private bool IsWithinBounds(Vector3 position)
    {
        
    }
}
