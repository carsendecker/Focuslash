using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallbearerScript : Enemy
{
    public float MaxDistanceFromPlayer;
    public float MinDistanceFromPlayer;
    
    [Space(10)] 
    public GameObject DeathParticles;

    private Vector3 direction;
    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        sr.gameObject.transform.rotation = Quaternion.identity;

        // if (Vector3.Distance(Services.Player.transform.position, transform.position) < MaxDistanceFromPlayer)
        // {
        //     rb.velocity = Vector2.zero;
        // }

        if (Services.Player.IsPhase(PlayerController.Phase.Attacking)) return;
        
        Vector3 targetPos = Services.Player.transform.position;
    
        direction = Vector3.Lerp(direction, targetPos - transform.position, 0.04f);
        rb.SetRotation(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    private void FixedUpdate()
    {
        if(Vector3.Distance(Services.Player.transform.position, transform.position) > MaxDistanceFromPlayer)
            rb.velocity = transform.right * MoveSpeed;
        
        else if (Vector3.Distance(Services.Player.transform.position, transform.position) < MinDistanceFromPlayer)
            rb.velocity = transform.right * -MoveSpeed;

        else
            rb.velocity = Vector2.zero;
    }
    
    
    protected override void Die()
    {
        base.Die();
        Instantiate(DeathParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
