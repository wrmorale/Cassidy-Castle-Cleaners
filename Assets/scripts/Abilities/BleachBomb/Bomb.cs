using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Projectile
{
    private Rigidbody body;
    [HideInInspector] public float distanceToTarget = 3.0f;
    [SerializeField] private Explosion explosion;
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player" && !other.isTrigger) 
        {
            Destroy(gameObject);
        }
    }
    public void launch() 
    {
        //Reach target horizontally in about 1 second
        //Travels about 3 units by default
        Vector3 force = (Vector3.up + transform.forward);
        force.Normalize();
        force *= speed;

        float adjustment = distanceToTarget / 3.0f;
        force.x *= adjustment;
        force.z *= adjustment;

        body.AddForce(force);
    }

    private void OnDestroy()
    {
        Explosion clone = Instantiate(explosion, transform);
        clone.Initialize(damage, stagger);
        clone.transform.SetParent(null);
    }
}
