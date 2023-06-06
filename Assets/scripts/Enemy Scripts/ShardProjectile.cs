using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardProjectile : MothProjectile
{
    //After a brief period, the hitbox will grow to its full size
    public Vector3 fullSize;
    public Vector3 fullCenter;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("increaseSize");
    }

    public IEnumerator increaseSize()
    {
        Debug.Log("Increasing size");
        yield return new WaitForSeconds(0.1f);
        BoxCollider collider = GetComponent<BoxCollider>();
        collider.center = fullCenter;
        collider.size = fullSize;
        Debug.Log("Finished");
    }

    private void OnDestroy()
    {
        if (UnityEngine.Random.value <= trashSpawnChance)
        {
            float groundOffset = 0.33f; // Adjust this value to control the height offset from the ground
            Vector3 groundPosition = new Vector3(transform.position.x, groundOffset, transform.position.z);
            if(dustPilePrefab != null){
                GameObject dustPile = Instantiate(dustPilePrefab, groundPosition, Quaternion.identity);
                dustPile.SetActive(true);
            }
            // Customize the dust pile prefab as needed
        }
    }
}
