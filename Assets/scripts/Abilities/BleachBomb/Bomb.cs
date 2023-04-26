using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Projectile
{
    private Rigidbody body;
    [HideInInspector] public Vector3 toTarget = Vector3.zero;
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
        
        if(toTarget != Vector3.zero)
        {
            float hDiscanceToTarget = new Vector3(toTarget.x, 0, toTarget.z).magnitude;
            float hAdjustment = hDiscanceToTarget / 3.0f;

            if(hAdjustment >= 1.0)
            {
                force.x *= hAdjustment;
                force.z *= hAdjustment;
            }
            else
            {
                force.y *= Mathf.Pow(hAdjustment, 2);
            }
        }

        body.AddForce(force);
    }

    private void OnDestroy()
    {
        Explosion clone = Instantiate(explosion, transform);
        clone.Initialize(damage, stagger);
        clone.transform.SetParent(null);
    }
}
