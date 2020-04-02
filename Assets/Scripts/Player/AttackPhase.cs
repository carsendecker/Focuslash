using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=============================================//
//---------------ATTACK PHASE------------------//
//=============================================//

/// <summary>
/// The state the player is in when they have finished targeting and is now executing their attacks
/// </summary>
public class AttackPhase : PlayerPhase
{
	private Vector3 dashTarget;
	
	private const float AttackMoveSpeed = 50f;
	private const float AttackTargetDistance = 1.5f; //Distance at which the player will stop moving towards a target
	private const float timeToPause = 0.05f;
	
	private bool hitTarget;
	private float pauseTimer;
	private bool pausing;
	
	private Collider2D pCol; //The player's main collider
	
	public AttackPhase(PlayerController owner)
	{
		player = owner;
	}
	
	public override void OnEnter()
	{
		pCol = player.GetComponent<Collider2D>();
		pCol.isTrigger = true;
		dashTarget = player.AttackPositionQueue.Dequeue();
	}

	public override void Update()
	{
		//After hitting the target and leaving its collider, lerp velocity down, then continue to next target. 
		//Gives a short pause between each dash into an enemy.
		if (pausing && pauseTimer > 0)
		{
			pauseTimer -= Time.deltaTime;
			if(pauseTimer <= 0)
				pausing = false;
		}
		else if (hitTarget)
		{
		}
		else
		{
			//If you are a certain distance away from the target, start to pause and remove it from the target queue.
			if (Vector3.Distance(player.transform.position, dashTarget) < AttackTargetDistance && player.AttackPositionQueue.Count > 0)
			{
				dashTarget = player.AttackPositionQueue.Dequeue();
				pausing = true;
			}
			else if (Vector3.Distance(player.transform.position, dashTarget) < 0.7f && player.AttackPositionQueue.Count == 0)
			{
				player.rb.velocity /= 2.5f;
				player.SetPhase(PlayerController.Phase.Movement);
			}
		}

	}
	
	public override void FixedUpdate()
	{
		if (pausing)
		{
			// player.rb.velocity = Vector2.zero;
			player.rb.velocity = Vector2.Lerp(player.rb.velocity, Vector2.zero, 0.55f);

			if (player.rb.velocity.magnitude <= 1.5f && pauseTimer <= 0)
			{
				// pauseTimer = timeToPause;
				player.rb.velocity = Vector2.zero;
				pauseTimer += timeToPause;
			}
		}
		else if (hitTarget)
		{
			player.rb.velocity = player.rb.velocity;
		}
		else
		{
			MoveTowardsTarget();
		}

	}

	public override void OnExit()
	{
		//Reenable the player's range detector and make the player solid again
		pCol.isTrigger = false;

		//Short pause of iFrames after attacking
		player.iFramesForSeconds(0.5f, false);
		player.AttackPositionQueue.Clear();
		
		Services.Events.Fire(new PlayerLeftAttackPhase());
	}

	public override void OnTriggerEnter2D(Collider2D col)
	{
		//If you hit a wall, get knocked out of attacking
		if (col.gameObject.CompareTag("Wall"))
		{
			Services.Utility.ShakeCamera(0.5f, 0.3f);
			Vector3 prevVel = player.rb.velocity;
			player.rb.velocity = Vector2.zero;
			player.rb.AddForce(-prevVel, ForceMode2D.Impulse);
			player.SetPhase(PlayerController.Phase.Movement);
		}
		//If you enter a creature, damage it then make particles and sounds and stuff
		else if (col.GetComponent<Creature>())
		{
			GameObject.Instantiate(player.AttackParticlesPrefab, col.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0, 360)));
			Services.Audio.PlaySound(player.attackSound, SourceType.CreatureSound);
			
			//If *any* collided object has a creature script attached on the way to the target, deal damage
			col.GetComponent<Creature>().TakeDamage(player.Damage);
		
			Services.Utility.ShakeCamera(0.15f, 0.25f);
		}
		
	}

	public override void OnTriggerExit2D(Collider2D col)
	{
	}

	//Sets velocity towards the target
	private void MoveTowardsTarget()
	{
		Vector3 direction = dashTarget - player.transform.position;
		player.rb.velocity = Vector3.Lerp(player.rb.velocity, direction.normalized * AttackMoveSpeed, 0.35f);
	}
}


