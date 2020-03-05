using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingerScript : Enemy
{
    public float AttackRange;
    public float AttacksPerSecond;
    public float BulletRange;

    [Space(10)] 
    public GameObject DeathParticles;
    public GameObject Bullet;

    private bool attacking;
    private Rigidbody2D playerRb;
    private Vector3 direction;
    
    // Start is called before the first frame update
    void Start()
    {
        playerRb = Services.Player.rb;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = Services.Player.transform.position + 
                            (Vector3) (playerRb.velocity.normalized);
        
        direction = Vector3.Lerp(direction, targetPos - transform.position, 0.1f);
        rb.SetRotation(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        
        if (Vector3.Distance(Services.Player.transform.position, transform.position) < AttackRange && !attacking)
        {
            
        }
    }

    private IEnumerator Attack()
    {
        Instantiate(Bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(1 / AttacksPerSecond);
    }
    
    protected override void Die()
    {
        base.Die();
        Instantiate(DeathParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
