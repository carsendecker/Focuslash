using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntScript : Creature
{
    public int Damage;
    public float knockbackForce;
    public float MoveLeadDistance;
    
    public GameObject DeathParticles;

    private GameObject player;
    private Rigidbody2D rb;
    private Rigidbody2D playerRb;
    private Vector3 direction;
    
    
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        Vector3 targetPos = player.transform.position + (Vector3) (playerRb.velocity.normalized * MoveLeadDistance);
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
