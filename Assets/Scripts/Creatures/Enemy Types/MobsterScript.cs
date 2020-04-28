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
        
        //I apologize this is literally the most useless code ever but it prevents me from having to go and change every sprite since they don't get spawned connected to the prefab aaaaaa
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr.gameObject.Equals(gameObject))
        {
            GameObject spriteObj = new GameObject("SpriteObject");
            spriteObj.transform.parent = transform;
            spriteObj.transform.localPosition = Vector3.zero;
            spriteObj.transform.localScale = Vector3.one;
            SpriteRenderer newSr = spriteObj.AddComponent<SpriteRenderer>();
            newSr.sprite = sr.sprite;
            newSr.enabled = true;
            newSr.color = Color.white;
            newSr.sortingLayerName = "Objects";


            sr.enabled = false;
            sr = newSr;
        }
    }

    protected override void Update()
    {
        base.Update();
        sr.gameObject.transform.rotation = Quaternion.identity;
    }

    void FixedUpdate()
    {
        Vector3 targetPos = Services.Player.transform.position + 
                            (Vector3) (playerRb.velocity.normalized * MoveLeadDistance);
        
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
