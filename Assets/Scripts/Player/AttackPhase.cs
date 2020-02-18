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
	public GameObject targetedEnemy;
	
	private const float AttackMoveSpeed = 41f;
	
	private bool hitTarget;
	private bool pausing;
	
	private Collider2D pCol;
	private CircleCollider2D pRangeCol;
	private Rigidbody2D playerRB;
	
	public AttackPhase(PlayerController owner)
	{
		player = owner;
	}
	
	public override void OnEnter()
	{
		targetedEnemy = player.EnemyAttackQueue[0];
		pCol = player.GetComponent<Collider2D>();
		pRangeCol = player.GetComponentsInChildren<CircleCollider2D>()[1];
		playerRB = player.GetComponent<Rigidbody2D>();
	}

	public override void Run()
	{
		//If there is no target but the queue still has things in it, stuff went wrong probably
		if (targetedEnemy == null && player.EnemyAttackQueue.Count > 0)
		{
			player.SetPhase(PlayerController.Phase.Movement);
			Debug.Log("Enemy is null");
			return;
		}

		//If you have not entered the target's collider yet, move towards it
		if (!hitTarget && targetedEnemy != null && !pausing)
		{
			pRangeCol.enabled = false;
			pCol.isTrigger = true;
			MoveTowardsEnemy();
		}
		//If you've hit the target but have not left its collider, stay at constant velocity
		else if (hitTarget)
			playerRB.velocity = playerRB.velocity;
		
		//After hitting the target and leaving its collider, lerp velocity down, then continue to next target. 
		//Gives a short pause between each dash into an enemy.
		else if (pausing)
		{
			playerRB.velocity = Vector2.Lerp(playerRB.velocity, Vector2.zero, 0.25f);
			
			if (playerRB.velocity.magnitude <= 1f)
				pausing = false;
		}
	}

	public override void OnExit()
	{
		pRangeCol.enabled = true;
		pCol.isTrigger = false;
		targetedEnemy = null;
		
		player.EnemyAttackQueue.Clear();
	}

	public override void OnTriggerEnter2D(Collider2D col)
	{
		//If you enter a creature, make particles and sounds and stuff
		if (col.gameObject.GetComponent<Creature>())
		{
			GameObject.Instantiate(player.AttackParticlesPrefab, col.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0, 360)));
			Services.Audio.PlaySound(player.attackSound, SourceType.CreatureSound);
		}
		//If you hit the actual targeted enemy, start to slow down
		if (col.gameObject.Equals(targetedEnemy))
			hitTarget = true;
	}

	public override void OnTriggerExit2D(Collider2D col)
	{
		if (pRangeCol.enabled) return;
		
		//If *any* collided object has a creature script attached on the way to the target, deal damage
		if (col.GetComponent<Creature>() != null)
		{
			//If enemy dies, remove all instances of it in the queue
			if (col.GetComponent<Creature>().TakeDamage(player.Damage))
				player.EnemyAttackQueue.RemoveAll(enemy => enemy.Equals(col.gameObject));
			
			Services.Utility.ShakeCamera(0.1f, 0.25f);
		}
		
		//Once exiting trigger of the targeted enemy, remove it from the queue and pause a sec
		if (hitTarget && col.gameObject.Equals(targetedEnemy))
		{
			hitTarget = false;
			pausing = true;
			player.EnemyAttackQueue.Remove(targetedEnemy);
		}
		
		//If there are enemies left in the queue, set the next one as the new target
		if (player.EnemyAttackQueue.Count > 0)
			targetedEnemy = player.EnemyAttackQueue[0];
		
		else if (player.EnemyAttackQueue.Count == 0)
			player.SetPhase(PlayerController.Phase.Movement);

		
	}

	//Sets velocity towards the target
	private void MoveTowardsEnemy()
	{
		Vector3 direction = targetedEnemy.transform.position - player.transform.position;
		playerRB.velocity = Vector3.Lerp(playerRB.velocity, direction.normalized * AttackMoveSpeed, 0.25f);
	}
}
