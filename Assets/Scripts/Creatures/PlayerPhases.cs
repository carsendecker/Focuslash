using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//========================================//
//          PHASE STATE CLASSES           //
//========================================//

/// <summary>
/// The abstract class to be used in the player state machine.
/// </summary>
public abstract class PlayerPhase
{
	public PlayerController player;
	
	/// <summary>
	/// Gets called whenever the player enters the state
	/// </summary>
	public abstract void OnEnter();
	
	/// <summary>
	/// Gets called every frame while the player is in the state (like Update())
	/// </summary>
	public abstract void Run();
	
	/// <summary>
	/// Gets called whenever the player exits the state
	/// </summary>
	public abstract void OnExit();

	public virtual void OnCollisionEnter2D(Collision2D col)
	{
	}

	public virtual void OnTriggerEnter2D(Collider2D col)
	{
	}

	public virtual void OnTriggerExit2D(Collider2D col)
	{
	}
}



//=============================================//
//---------------MOVEMENT PHASE----------------//
//=============================================//

/// <summary>
/// The state the player is in whenever they are just moving around the map
/// </summary>
public class MovePhase : PlayerPhase
{
	private float cdTimer;

	public MovePhase(PlayerController owner)
	{
		player = owner;
	}
	
	public override void OnEnter()
	{
		SystemsManager.UI.CooldownSlider.maxValue = player.CooldownTime;
		if (player.coolingDown)
		{
			cdTimer = 0;
		}
		else
		{
			cdTimer = player.CooldownTime;
		}
		SystemsManager.UI.CooldownSlider.value = cdTimer;
		player.GetComponentsInChildren<SpriteRenderer>()[1].enabled = false;

		//Short pause of iFrames after attacking
		player.iFramesForSeconds(0.5f, true);
	}

	public override void Run()
	{
		if(player.canMove)
			player.Move();
		
		if (cdTimer < player.CooldownTime)
		{
			cdTimer += Time.deltaTime;
			SystemsManager.UI.CooldownSlider.value = cdTimer;
			
			if (cdTimer >= player.CooldownTime)
			{
				player.coolingDown = false;
			}
		}

		//TODO: Force player into the attack phase instead of them choosing when?
		if (!player.coolingDown && InputManager.PressedDown(Inputs.Attack))
		{
			player.SetPhase(PlayerController.Phase.Choosing);
		}
	}

	public override void OnExit()
	{
	}
}


			
//=============================================//
//---------------CHOOSE PHASE------------------//
//=============================================//

public class ChoosePhase : PlayerPhase
{		
	private GameObject targetedEnemy;
	private GameObject crosshair;
	private int optionSelected = 0;
	private int attacksLeft;
	private bool confirmingAttack;
	
	private Color camColor;
	private List<GameObject> crosshairList = new List<GameObject>();
	
	public ChoosePhase(PlayerController owner)
	{
		player = owner;
	}
	
	public override void OnEnter()
	{
		//Slows down time and such
		Time.timeScale = 0.05f;
		camColor = Camera.main.backgroundColor;
		Camera.main.backgroundColor = player.SlowMoColor;
		SystemsManager.Audio.PlaySound(player.enterSlomoSound, SourceType.PlayerSound);
		
		player.GetComponent<Rigidbody2D>().velocity /= 5;
		
		SystemsManager.UI.TimelineSlider.gameObject.SetActive(true);
		SystemsManager.UI.TimelineSlider.maxValue = player.AttackCount;
		
		attacksLeft = player.AttackCount;

		//Turns on range indicator sprite
		player.GetComponentsInChildren<SpriteRenderer>()[1].enabled = true;

		//Target the closest enemy in range first
		GameObject closestEnemy = null;
		float closestDistance = 999999;
		foreach (var enemy in player.EnemyList)
		{	
			float distance = Vector2.Distance(player.transform.position, enemy.transform.position);
			if (distance < closestDistance)
			{
				distance = closestDistance;
				closestEnemy = enemy;
			}
		}

		if (closestEnemy == null)
		{
			crosshair = null;
			targetedEnemy = null;
			return;
		}
		targetedEnemy = closestEnemy;
		crosshair = GameObject.Instantiate(player.CrosshairPrefab, closestEnemy.transform.position, Quaternion.identity);
	}

	//Runs every update frame
	public override void Run()
	{
		SystemsManager.UI.TimelineSlider.value = Mathf.Lerp(SystemsManager.UI.TimelineSlider.value, player.EnemyQueue.Count, 0.25f);
		if (crosshair != null)
		{
			CycleEnemies();
			crosshair.transform.position = Vector3.Lerp(crosshair.transform.position, targetedEnemy.transform.position, 0.25f);
		}

		if (InputManager.PressedDown(Inputs.Attack))
		{
			if (confirmingAttack)
			{
				//Once pressed again after targeting everything, start attacking
				SystemsManager.UI.TimelineInstructionText.gameObject.SetActive(false);
				player.SetPhase(PlayerController.Phase.Attacking);
				return;
			}

			if (targetedEnemy == null || crosshair == null)
			{
				player.SetPhase(PlayerController.Phase.Movement);
				return;
			}
			
			//Adds the targeted enemy to a "queue" for later
			player.EnemyQueue.Add(targetedEnemy);

			//Spawns a new crosshair to show the enemy has been targeted
			if (player.EnemyQueue.Contains(targetedEnemy))
			{
				GameObject targetCrosshair = GameObject.Instantiate(player.LockedCrosshairPrefab,targetedEnemy.transform.position, Quaternion.identity);
				targetCrosshair.transform.parent = targetedEnemy.transform;
				crosshairList.Add(targetCrosshair);
				
				SystemsManager.Audio.PlaySound(player.selectTargetSound, SourceType.PlayerSound);

				attacksLeft--;

				//If there are no more attacks left, remove the crosshair and show a confirmation
				if (attacksLeft <= 0)
				{
					confirmingAttack = true;
					GameObject.Destroy(crosshair);
					SystemsManager.UI.TimelineInstructionText.gameObject.SetActive(true);
				}
			}
		}

		if (InputManager.PressedDown(Inputs.Cancel))
		{
			//Not working rn
			
//			player.EnemyQueue.RemoveAt(player.EnemyQueue.Count - 1);
//			GameObject.Destroy(crosshairList[crosshairList.Count - 1]);
//			crosshairList.RemoveAt(crosshairList.Count - 1);
//
//			targetedEnemy = player.EnemyQueue[player.EnemyQueue.Count - 1];
//			if (confirmingAttack)
//			{
//				crosshair = GameObject.Instantiate(player.CrosshairPrefab, targetedEnemy.transform.position, Quaternion.identity);
//				confirmingAttack = false;
//			}
//			
//			if (player.EnemyQueue.Count == 0)
//			{
//				MenuGod.MG.TimelineSlider.gameObject.SetActive(false);
//				player.SetPhase(PlayerController.Phase.Movement);
//			}
			
			player.EnemyQueue.Clear();
			SystemsManager.UI.TimelineSlider.gameObject.SetActive(false);
			player.SetPhase(PlayerController.Phase.Movement);
		}

//		ChooseAttack();
	}

	public override void OnExit()
	{
		Time.timeScale = 1f;
		Camera.main.backgroundColor = camColor;
		
		GameObject.Destroy(crosshair);
		crosshair = null;
		
		targetedEnemy = null;
		confirmingAttack = false;
		//Disable circle range sprite
		player.GetComponentsInChildren<SpriteRenderer>()[1].enabled = false;
		
//		MenuGod.MG.AttackPanel.SetActive(false);

		//Get rid of all target crosshairs
		foreach (var thing in crosshairList)
		{ 
			GameObject.Destroy(thing);
		}
		crosshairList.Clear();
	}

	//Takes input left and right to cycle through enemies in range
	//TODO: Better targeting (fix distance based thing?) 
	private void CycleEnemies()
	{
		int currentIndex = player.EnemyList.IndexOf(targetedEnemy);
		if (InputManager.PressedDown(Inputs.Left))
		{
			if (currentIndex > 0)
			{
				currentIndex--;
			}
			else
			{
				currentIndex = player.EnemyList.Count - 1;
			}
			targetedEnemy = player.EnemyList[currentIndex];
			SystemsManager.Audio.PlaySound(player.moveTargetSound, SourceType.PlayerSound);
		}
		else if (InputManager.PressedDown(Inputs.Right))
		{
			if (currentIndex < player.EnemyList.Count - 1)
			{
				currentIndex++;
			}
			else
			{
				currentIndex = 0;
			}
			targetedEnemy = player.EnemyList[currentIndex];
			SystemsManager.Audio.PlaySound(player.moveTargetSound, SourceType.PlayerSound);
		}
	}
}



//=============================================//
//---------------ATTACK PHASE------------------//
//=============================================//

public class AttackPhase : PlayerPhase
{
//	public List<>/Dictionary<> attackQueue
	public GameObject targetedEnemy;
	
	private const float attackMoveSpeed = 40f;
	private bool hitTarget;
	private Collider2D pCol;
	private CircleCollider2D pRangeCol;
	private bool pausing;
	
	public AttackPhase(PlayerController owner)
	{
		player = owner;
	}
	
	public override void OnEnter()
	{
		targetedEnemy = player.EnemyQueue[0];
		pCol = player.GetComponent<Collider2D>();
		pRangeCol = player.GetComponentsInChildren<CircleCollider2D>()[1];
	}

	public override void Run()
	{
		SystemsManager.UI.TimelineSlider.value = Mathf.Lerp(SystemsManager.UI.TimelineSlider.value, player.EnemyQueue.Count, 0.3f);

		//If there is no target but the queue still has things in it, stuff went wrong probably
		if (targetedEnemy == null && player.EnemyQueue.Count > 0)
		{
			player.SetPhase(PlayerController.Phase.Movement);
			Debug.Log("Enemy is null");
			return;
		}

		if (!hitTarget && targetedEnemy != null && !pausing)
		{
			//If you have not entered the target's collider yet, move towards it
			pRangeCol.enabled = false;
			pCol.isTrigger = true;
			MoveTowardsEnemy();
		}
		else if (hitTarget)
		{
			//If you've hit the target but have not left its collider, stay at constant velocity
			player.rb.velocity = player.rb.velocity;
		}
		else if (pausing)
		{
			//After hitting the target and leaving its collider, lerp velocity down, then continue to next target. Gives a short pause
			//between each dash into an enemy.
			
			//**Lerp speed and magnitude comparison effect speed of pause**
			player.rb.velocity = Vector2.Lerp(player.rb.velocity, Vector2.zero, 0.3f);
			if (player.rb.velocity.magnitude <= 1.5f)
			{
				pausing = false;
			}
		}
	}

	public override void OnExit()
	{
		player.coolingDown = true;
		pRangeCol.enabled = true;
		pCol.isTrigger = false;
		targetedEnemy = null;
		
		player.EnemyQueue.Clear();
		SystemsManager.UI.TimelineSlider.gameObject.SetActive(false);
	}

	public override void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.GetComponent<Creature>())
		{
			GameObject.Instantiate(player.AttackParticlesPrefab, col.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0, 360)));
			SystemsManager.Audio.PlaySound(player.attackSound, SourceType.PlayerSound);
		}
		if (col.gameObject.Equals(targetedEnemy))
		{
			hitTarget = true;
		}
	}

	public override void OnTriggerExit2D(Collider2D col)
	{
		if (pRangeCol.enabled) return;
		//Once exiting trigger of the targeted enemy, remove it from the queue and pause a sec
		if (hitTarget && col.gameObject.Equals(targetedEnemy))
		{
			hitTarget = false;
			pausing = true;
			player.EnemyQueue.Remove(targetedEnemy);
//			player.rb.velocity /= 3;
		}
		
		//If *any* collided object has a creature script attached on the way to the target, deal damage
		if (col.GetComponent<Creature>() != null)
		{
			//If enemy dies, remove all instances of it in the queue
			if (col.GetComponent<Creature>().TakeDamage(player.AttackDamage))
			{
				player.EnemyQueue.RemoveAll(enemy => enemy.Equals(col.gameObject));
			}
			SystemsManager.Utility.ShakeCamera(0.1f, 0.25f);
		}
		
		//If there are enemies left in the queue, set the next one as the new target
		if (player.EnemyQueue.Count > 0)
		{
			targetedEnemy = player.EnemyQueue[0];
		}
		else if (player.EnemyQueue.Count == 0)
		{
			player.SetPhase(PlayerController.Phase.Movement);
		}

		
	}

	//Sets velocity towards the target
	private void MoveTowardsEnemy()
	{
		Vector3 direction = targetedEnemy.transform.position - player.transform.position;
		player.rb.velocity = Vector3.Lerp(player.rb.velocity, direction.normalized * attackMoveSpeed, 0.25f);
	}
}