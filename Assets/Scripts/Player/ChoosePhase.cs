using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
//	private int optionSelected = 0;
//	private bool confirmingAttack;
	private float startingFocus; //How much focus you start choosing with, used for when you cancel your attack
	
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
		startingFocus = player.CurrentFocus;
		
		player.GetComponent<Rigidbody2D>().velocity /= 5;
		
//		Services.UI.TimelineSlider.gameObject.SetActive(true);
		
		//Turns on range indicator sprite
		player.GetComponentsInChildren<SpriteRenderer>()[1].enabled = true;
		Services.UI.AttackInstructionText.gameObject.SetActive(true);

		targetedEnemy = null;

	}

	//Runs every update frame
	public override void Run()
	{
		if (InputManager.PressedDown(Inputs.Attack))
		{
			if (player.EnemyAttackQueue.Count == 0)
			{
				player.SetPhase(PlayerController.Phase.Movement);
				return;
			}
			
			//Once pressed again after targeting everything, start attacking
			player.SetPhase(PlayerController.Phase.Attacking);
			return;
		}
		
		if (player.CurrentFocus < 1)
		{
			if(crosshair != null)
				GameObject.Destroy(crosshair);
			
			return;
		}

		TargetWithMouse();


		if (InputManager.PressedDown(Inputs.Target) && targetedEnemy != null)
		{
			//Adds the targeted enemy to a "queue" for later
			player.EnemyAttackQueue.Add(targetedEnemy);

			//Spawns a new crosshair to show the enemy has been targeted
			GameObject targetCrosshair = GameObject.Instantiate(player.LockedCrosshairPrefab,targetedEnemy.transform.position, Quaternion.identity);
			targetCrosshair.transform.parent = targetedEnemy.transform;
			crosshairList.Add(targetCrosshair);
			
			Services.Audio.PlaySound(player.selectTargetSound, SourceType.CreatureSound);

			player.CurrentFocus--;
		}
		
		if (InputManager.PressedDown(Inputs.Cancel))
		{
			//Cancels the current attack
			player.EnemyAttackQueue.Clear();
			player.CurrentFocus = startingFocus;
			player.SetPhase(PlayerController.Phase.Movement);
		}


	}

	public override void OnExit()
	{
		Services.UI.AttackInstructionText.gameObject.SetActive(false);

		Time.timeScale = 1f;
		Camera.main.backgroundColor = camColor;

		if (crosshair != null)
		{
			GameObject.Destroy(crosshair);
			crosshair = null;
		}

		targetedEnemy = null;
		//Disable circle range sprite
		player.GetComponentsInChildren<SpriteRenderer>()[1].enabled = false;
		

		//Get rid of all target crosshairs
		foreach (var thing in crosshairList)
		{ 
			GameObject.Destroy(thing);
		}
		crosshairList.Clear();
	}


	private void TargetWithMouse()
	{
		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero, 0f);

		foreach (RaycastHit2D hit in hits)
		{
			GameObject creature = hit.collider.gameObject;
			
			//If the target isn't a Creature, dont target it
			if (!creature.GetComponent<Creature>()) continue;

			//If the enemy isnt in range, don't target it
			if(!player.EnemiesInRange.Contains(creature)) continue;

			//If the mouse is already on the targeted enemy, just update the crosshairs position then return
			if (creature.Equals(targetedEnemy))
			{
				crosshair.transform.position = targetedEnemy.transform.position;
				return;
			}
			
			//If the mouse is on a new enemy, make that creature the new target and make a new crosshair on it
			targetedEnemy = creature;
			GameObject.Destroy(crosshair);
			crosshair = GameObject.Instantiate(player.CrosshairPrefab, targetedEnemy.transform.position, Quaternion.identity);
			
			Services.Audio.PlaySound(player.moveTargetSound, SourceType.CreatureSound);
			
			return;
		}

		targetedEnemy = null;
		GameObject.Destroy(crosshair);
	}
}
