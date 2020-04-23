using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float MoveSpeed;
    public int Damage;

    private Rigidbody2D rb;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        rb.velocity = transform.right * MoveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !Services.Player.IsPhase(PlayerController.Phase.Attacking))
        {
            Services.Player.TakeDamage(Damage);
            Kill();
        }
        else if (other.CompareTag("Wall"))
        {
            Kill();
        }
    }

    private void Kill()
    {
        //TODO: Check if the bullet is part of an object pool. If it isn't, destroy it
        gameObject.SetActive(false);
        // if(Services.ObjectPools.Pools.ContainsKey(gameObject.name.Substring()))
    }
}
