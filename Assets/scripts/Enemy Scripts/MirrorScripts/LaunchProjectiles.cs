using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchProjectiles : MonoBehaviour
{
    public GameObject projectile;
    // Start is called before the first frame update
    void Start()
    {
        
        Instantiate(projectile, transform.position, transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
