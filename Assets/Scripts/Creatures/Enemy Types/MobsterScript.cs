using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobsterScript : Enemy
{
    public float knockbackForce;
    public float MoveLeadDistance;
    public float AttackRange;
    public float AttackLungeForce;
    public float AttackWindupTime;
    public float AttackRestTime;
    
    public GameObject DeathParticles;

    private Vector3 direction;
    private Rigidbody2D playerRb;
    private bool inAttackRange;
    private bool attacking;
    private SpriteRenderer sr;
    
    
    protected override void Start()
    {
        base.Start();
        playerRb = Services.Player.GetComponent<Rigidbody2D>();
        
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void Update()
    {
        base.Update();
    }

    void FixedUpdate()
    {
        Vector3 targetPos = Services.Player.transform.position + (Vector3) (playerRb.velocity.normalized * MoveLeadDistance);
        
        direction = Vector3.Lerp(direction, targetPos - transform.position, 0.1f);
        rb.SetRotation(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        
        if (Vector3.Distance(Services.Player.transform.position, transform.position) > AttackRange && !attacking)
        {
            //Runs towards player, but not quite perfectly
            rb.velocity = transform.right * MoveSpeed;
        }
        else if (!attacking)
        {
            StartCoroutine(Attack());
        }
        else
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 0.2f);
        }
    }

    IEnumerator Attack()
    {
        attacking = true;
        yield return new WaitForSeconds(AttackWindupTime);
        
        rb.AddForce(transform.right * AttackLungeForce, ForceMode2D.Impulse);
        
        yield return new WaitForSeconds(AttackRestTime);
        attacking = false;
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
        base.Die();
        Instantiate(DeathParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
