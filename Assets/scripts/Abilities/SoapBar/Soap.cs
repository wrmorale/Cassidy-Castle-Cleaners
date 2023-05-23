using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soap : Projectile
{
    // Maximum number of bounces
    public const int maxBounces = 5;

    // Current number of bounces
    private int bounceCount = 0;

    // Radius of the sphere for the SphereCast
    private const float sphereRadius = 0.1f;

    // Distance the SphereCast will check for collisions
    private const float sphereCastDistance = 0.1f;

    protected override void Update()
    {
        base.Update();

        // Use a SphereCast to check for collisions in the direction the projectile is moving
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, sphereRadius, heading, out hit, sphereCastDistance))
        {
            // If the projectile hit an enemy, damage the enemy and destroy the projectile
            if (hit.collider.tag == "Enemy")
            {
                Debug.Log("projectile hit");
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                enemy.isHit(damage, stagger);
            }
            else if(hit.collider.tag == "DustPile")
            {
                Debug.Log("projectile hit");
                DustPile dustPile = hit.collider.GetComponent<DustPile>();
                dustPile.isHit(damage);
            }
            // If the projectile hit something else, reflect the heading
            else if(hit.collider.tag != "Enemy")
            {
                // Increment the bounce count
                bounceCount++;

                // If the projectile has bounced the maximum number of times, destroy it
                if (bounceCount >= maxBounces)
                {
                    Destroy(gameObject);
                }
                else
                {
                    // Calculate the new heading on the ZX plane
                    Vector3 surfaceNormal = hit.normal;
                    Vector3 reflectDirection = Vector3.Reflect(heading, surfaceNormal);
                    Vector3 newHeading = new Vector3(reflectDirection.x, 0f, reflectDirection.z).normalized;

                    // Update the heading
                    heading = newHeading;
                }
            }
        }

        transform.position += heading * speed * Time.deltaTime;
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy") 
        {
            Debug.Log("projectile hit");
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.isHit(damage, stagger);
            //Destroy(gameObject);
        }
    }*/

    private void OnCollisionEnter(Collision collision)
    {
        // Increment the bounce count
        bounceCount++;

        // If the projectile has bounced the maximum number of times, destroy it
        if (bounceCount >= maxBounces)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Debug.Log("projectile destroyed");
    }
}
