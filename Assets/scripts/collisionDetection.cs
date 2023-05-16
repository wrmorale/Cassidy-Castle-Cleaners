using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using States;

public class collisionDetection : MonoBehaviour
{
    public playerController pc;
    public Player player;
    public Enemy enemy;
    [HideInInspector] public float damage = 1; //Set by enemys' attack manager
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Enemy enemyEx = other.GetComponent<Enemy>();
            if (enemyEx != null && pc != null && pc.state == States.PlayerStates.Attacking)
            {
                enemyEx.isHit(player.basicDamage, player.staggerDamage);
            }
        }

        if (other.tag == "DustPile")
        {
            DustPile dustPile = other.GetComponent<DustPile>();
            if (dustPile != null && pc != null && pc.state == States.PlayerStates.Attacking)
            {
                dustPile.isHit(player.basicDamage);
            }
        }

        if (other.tag == "Furniture")
        {
            Furniture furniture = other.GetComponent<Furniture>();
            if (furniture != null && pc != null && pc.state == States.PlayerStates.Attacking)
            {
                furniture.isHit(player.basicDamage);
            }
        }
        
        if (other.tag == "Player" && this.tag != "weapon" && this.tag != "Projectile")
        {
            Player playerEx = other.GetComponent<Player>();
            if (playerEx != null)
            {
                playerEx.isHit(damage);
            }
        }
    }
}
