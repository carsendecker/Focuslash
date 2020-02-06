using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=============================================//
//---------------CHOOSE PHASE------------------//
//=============================================//

/// <summary>
/// The state the player is in whenever they are focusing and targeting enemies to attack
/// </summary>
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
		Services.Audio.PlaySound(player.enterSlomoSound, SourceType.CreatureSound);
		
		player.GetComponent<Rigidbody2D>().velocity /= 5;
		
		Services.UI.TimelineSlider.gameObject.SetActive(true);
		Services.UI.TimelineSlider.maxValue = player.AttackCount;
		
		attacksLeft = player.AttackCount;

		//Turns on range indicator sprite
		player.GetComponentsInChildren<SpriteRenderer>()[1].enabled = true;

		//Target the closest enemy in range first
		GameObject closestEnemy = null;
		float closestDistance = 999999;
		foreach (var enemy in player.EnemiesInRange)
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
		Services.UI.TimelineSlider.value = Mathf.Lerp(Services.UI.TimelineSlider.value, player.EnemyAttackQueue.Count, 0.25f);
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
				Services.UI.TimelineInstructionText.gameObject.SetActive(false);
				player.SetPhase(PlayerController.Phase.Attacking);
				return;
			}

			if (targetedEnemy == null || crosshair == null)
			{
				player.SetPhase(PlayerController.Phase.Movement);
				return;
			}
			
			//Adds the targeted enemy to a "queue" for later
			player.EnemyAttackQueue.Add(targetedEnemy);

			//Spawns a new crosshair to show the enemy has been targeted
			if (player.EnemyAttackQueue.Contains(targetedEnemy))
			{
				GameObject targetCrosshair = GameObject.Instantiate(player.LockedCrosshairPrefab,targetedEnemy.transform.position, Quaternion.identity);
				targetCrosshair.transform.parent = targetedEnemy.transform;
				crosshairList.Add(targetCrosshair);
				
				Services.Audio.PlaySound(player.selectTargetSound, SourceType.CreatureSound);

				attacksLeft--;

				//If there are no more attacks left, remove the crosshair and show a confirmation
				if (attacksLeft <= 0)
				{
					confirmingAttack = true;
					GameObject.Destroy(crosshair);
					Services.UI.TimelineInstructionText.gameObject.SetActive(true);
				}
			}
		}

		if (InputManager.PressedDown(Inputs.Cancel))
		{
			//Cancels the current attack
			player.EnemyAttackQueue.Clear();
			Services.UI.TimelineSlider.gameObject.SetActive(false);
			player.SetPhase(PlayerController.Phase.Movement);
		}

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
		int currentIndex = player.EnemiesInRange.IndexOf(targetedEnemy);
		if (InputManager.PressedDown(Inputs.Left))
		{
			if (currentIndex > 0)
			{
				currentIndex--;
			}
			else
			{
				currentIndex = player.EnemiesInRange.Count - 1;
			}
			targetedEnemy = player.EnemiesInRange[currentIndex];
			Services.Audio.PlaySound(player.moveTargetSound, SourceType.CreatureSound);
		}
		else if (InputManager.PressedDown(Inputs.Right))
		{
			if (currentIndex < player.EnemiesInRange.Count - 1)
			{
				currentIndex++;
			}
			else
			{
				currentIndex = 0;
			}
			targetedEnemy = player.EnemiesInRange[currentIndex];
			Services.Audio.PlaySound(player.moveTargetSound, SourceType.CreatureSound);
		}
	}
}
