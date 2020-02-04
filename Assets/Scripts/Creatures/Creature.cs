using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class for all "living" entities in the game, includes enemies, the player, etc
/// </summary>
public class Creature : MonoBehaviour
{

    public int MaxHealth; 
    public float MoveSpeed;

    protected int health;


    public virtual void Start()
    {
        health = MaxHealth;
    }
    
    /// <summary>
    /// For making a creature aggressive towards another.
    /// 
    /// Call this from the creature that *will be aggro'd onto*
    /// (ie call this from the player when entering enemy range [enemy.Aggro(true) or something])
    /// </summary>
    public virtual void Aggro(bool aggrod)
    {
        this.enabled = aggrod;
    }

    //Call this from the creature DEALING the damage (so like objectToDamage.TakeDamage())
    //If the object taking damage is killed, returns a value of true
    public virtual bool TakeDamage(int damage)
    {
        health -= damage;
        
        if (health <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    //For when a creature dies
    protected virtual void Die() {}

    
}


