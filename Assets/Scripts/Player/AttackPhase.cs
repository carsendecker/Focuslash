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
//	public List<>/Dictionary<> attackQueue
	private Vector3 dashTarget;
	
	private const float AttackMoveSpeed = 50f;
	private const float AttackTargetDistance = 1f; //Distance at which the player will stop moving towards a target
	private const float timeToPause = 0.1f;
	
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
			// pausing = false;
		}
		//If you're not pausing and you hit the target, but have not left its collider, stay at constant velocity
		else if (hitTarget)
		{
			player.rb.velocity = player.rb.velocity;
		}
		else
		{
			//If you aren't pausing and have not entered the target's collider yet, move towards it
			if (Vector3.Distance(player.transform.position, dashTarget) < AttackTargetDistance)
			{
				if (player.AttackPositionQueue.Count == 0)
				{
					player.SetPhase(PlayerController.Phase.Movement);
					return;
				}

				dashTarget = player.AttackPositionQueue.Dequeue();
				pausing = true;
			}
		}

	}
	
	public override void FixedUpdate()
	{
		if (pausing)
		{
			// player.rb.velocity = Vector2.zero;
			player.rb.velocity = Vector2.Lerp(player.rb.velocity, Vector2.zero, 0.55f);

			if (player.rb.velocity.magnitude <= 2f)
			{
				// pauseTimer = timeToPause;
				player.rb.velocity = Vector2.zero;
				pausing = false;
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
		player.iFramesForSeconds(0.6f, false);
	}

	public override void OnTriggerEnter2D(Collider2D col)
	{
		//If you enter a creature, make particles and sounds and stuff
		if (col.GetComponent<Creature>())
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
