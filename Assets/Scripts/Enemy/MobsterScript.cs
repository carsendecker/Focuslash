using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobsterScript : Creature
{
    public float knockbackForce;
    public float MoveLeadDistance;
    
    public GameObject DeathParticles;

    private Vector3 direction;
    
    
    protected override void Start()
    {
        base.Start();
    }

    void FixedUpdate()
    {
        //Runs towards player, but not quite perfectly
        Vector3 targetPos = Services.Player.transform.position + 
                            (Vector3) (Services.Player.GetComponent<Rigidbody2D>().velocity.normalized * MoveLeadDistance);
        
        direction = Vector3.Lerp(direction, targetPos - transform.position, 0.1f);
        rb.SetRotation(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        rb.velocity = transform.right * MoveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(Damage, knockbackForce, transform);
        }
    }

    protected override void Die()
    {
        Instantiate(DeathParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
