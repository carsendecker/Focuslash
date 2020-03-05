using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class for all "living" entities in the game, includes enemies, the player, etc
/// </summary>
public class Creature : MonoBehaviour
{

    [Tooltip("This creature's max health points.")]
    public int MaxHealth; 
    
    [Tooltip("This creature's movement speed. Self explanatory.")]
    public float MoveSpeed;
    
    [Tooltip("The *base* damage this creature deals if it attacks another creature.")]
    public int Damage;

    protected float health;
    [HideInInspector] public Rigidbody2D rb;


    protected virtual void Start()
    {
        health = MaxHealth;
        if (GetComponent<Rigidbody2D>() != null)
            rb = GetComponent<Rigidbody2D>();
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

    /// <summary>
    /// Returns the creature's current health
    /// </summary>
    public float GetHealth()
    {
        return health;
    }
    
    public virtual void Heal(int amountToHeal = 999)
    {
        health += amountToHeal;
        if (health > MaxHealth) 
            health = MaxHealth;
    }

    
}


