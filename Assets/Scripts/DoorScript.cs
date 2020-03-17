using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : Enemy
{
   // public float knockbackForce;
   // public float MoveLeadDistance;
   // public float AttackRange;
   // public float AttackLungeForce;
  //  public float AttackWindupTime;
  //  public float AttackRestTime;
    
  public GameObject DeathParticles;
  private SpriteRenderer thisSpriteRenderer;

   // private Vector3 direction;
    //private Rigidbody2D playerRb;
   // private bool inAttackRange;
   // private bool attacking;
   
   //This part of the script will LISTEN for when the event has occured
   public void Start()
   {
       SetHealth(999);
       thisSpriteRenderer = GetComponent<SpriteRenderer>();
       thisSpriteRenderer.color = Color.gray;
 
       //Add the method to the event currently on the DoorEvents script
       //The video I'm using called it subscribing
       DoorEvents.current.onDoorwayTriggerEnter += closeDoorWay;
       DoorEvents.current.onEnemiesDefeated += makeDoorSlashable;
       
       //Set the door to be inactive.
       this.gameObject.SetActive(false);
       Debug.Log("Close Door");
   }

   public void closeDoorWay()
   {
       //Set the game object to active here.
        this.gameObject.SetActive(true);
   }

   public void makeDoorSlashable()
   {
       thisSpriteRenderer.color = Color.blue;
       SetHealth(1);
   }
   
   

   protected override void Die()
   {
       base.Die();
       Instantiate(DeathParticles, transform.position, Quaternion.identity);
       Destroy(gameObject);
   }
    
    
    /*protected override void Start()
    {
        base.Start();
        playerRb = Services.Player.GetComponent<Rigidbody2D>();
    }*/

    /*/*void FixedUpdate()
    {
        /* Vector3 targetPos = Services.Player.transform.position + 
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
        }#2##1#
    }*/

    /*IEnumerator Attack()
    {
        /*attacking = true;
        yield return new WaitForSeconds(AttackWindupTime);
        
        rb.AddForce(transform.right * AttackLungeForce, ForceMode2D.Impulse);
        
        yield return new WaitForSeconds(AttackRestTime);
        attacking = false;#1#
    }*/

    /*private void OnCollisionEnter2D(Collision2D other)
    {
        /*if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(Damage, knockbackForce, transform);
        }#1#
    }*/

   
}
