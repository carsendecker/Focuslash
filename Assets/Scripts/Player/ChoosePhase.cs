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
	private LineRenderer attackLine;
	private Vector3 attackLineStartPos;
	private float startingFocus; //How much focus you start choosing with, used for when you cancel your attack
	
	private Color camColor;
	private List<GameObject> crosshairList = new List<GameObject>();
	private List<GameObject> attackLineList = new List<GameObject>();
	private Vector3[] linePositions;
	
	public ChoosePhase(PlayerController owner)
	{
		player = owner;
	}
	
	public override void OnEnter()
	{
		//Slows down time and such
		Time.timeScale = 0.04f;
		camColor = Camera.main.backgroundColor;
		Camera.main.backgroundColor = player.SlowMoColor;
		Services.Audio.PlaySound(player.enterSlomoSound, SourceType.CreatureSound);
		startingFocus = player.CurrentFocus;

		player.GetComponent<Rigidbody2D>().velocity /= 5;

		//Turns on range indicator sprite
		player.GetComponentsInChildren<SpriteRenderer>()[1].enabled = true;
		Services.UI.AttackInstructionText.gameObject.SetActive(true);

		targetedEnemy = null;

		CreateNewLine(player.transform.position);
	}

	//Runs every update frame
	public override void Run()
	{
		if (InputManager.PressedUp(Inputs.Focus))
		{
			if (player.AttackPositionQueue.Count == 0)
			{
				SetTarget();
			}
			
			//Once pressed again after targeting everything, start attacking
			player.SetPhase(PlayerController.Phase.Attacking);
			return;
		}
		
		if (InputManager.PressedDown(Inputs.Cancel))
		{
			//Cancels the current attack
			player.AttackPositionQueue.Clear();
			player.CurrentFocus = startingFocus;
			player.SetPhase(PlayerController.Phase.Movement);
			return;
		}
		
		if (player.CurrentFocus < 1)
		{
			if(crosshair != null)
				GameObject.Destroy(crosshair);
			
			return;
		}

		// TargetWithMouse();
		DrawAttackLine();

		if (InputManager.PressedDown(Inputs.Target))
		{
			//Adds the targeted enemy to a "queue" for later
			// player.EnemyAttackQueue.Add(targetedEnemy);
			

			//Spawns a new crosshair to show the enemy has been targeted
			// GameObject targetCrosshair = GameObject.Instantiate(player.LockedCrosshairPrefab,targetedEnemy.transform.position,Quaternion.identity);
			// targetCrosshair.transform.parent = targetedEnemy.transform;
			// crosshairList.Add(targetCrosshair);
			
			Services.Audio.PlaySound(player.selectTargetSound, SourceType.CreatureSound);

			if (player.CurrentFocus >= 1)
			{
				CreateNewLine(attackLine.GetPosition(1));
			}
		}

	}

	public override void OnExit()
	{
		Services.UI.AttackInstructionText.gameObject.SetActive(false);

		Time.timeScale = 1f;
		Services.MainCamera.backgroundColor = camColor;

		if (crosshair != null)
		{
			GameObject.Destroy(crosshair);
			crosshair = null;
		}

		targetedEnemy = null;
		//Disable circle range sprite
		player.GetComponentsInChildren<SpriteRenderer>()[1].enabled = false;

		foreach (GameObject line in attackLineList)
		{
			GameObject.Destroy(line);
		}
		attackLineList.Clear();
		
		//Get rid of all target crosshairs
		foreach (var thing in crosshairList)
		{ 
			GameObject.Destroy(thing);
		}
		crosshairList.Clear();
	}

	private void SetTarget()
	{
		player.AttackPositionQueue.Enqueue(attackLine.GetPosition(1));
		player.CurrentFocus--;
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
	
	/// <summary>
	/// Takes the current line and changes its end position to the player's mouse position
	/// </summary>
	private void DrawAttackLine()
	{
		Vector2 mousePos = Services.MainCamera.ScreenToWorldPoint(Input.mousePosition);
		float distanceFromPlayer = Mathf.Clamp(Vector2.Distance(attackLineStartPos, mousePos), 0, player.AttackLineRange);

		Vector2 lineEndPos = (mousePos - (Vector2) attackLineStartPos).normalized * distanceFromPlayer;
		
		attackLine.SetPosition(0, attackLineStartPos);
		attackLine.SetPosition(1, (Vector2)attackLineStartPos + lineEndPos);
	}
	

	/// <summary>
	/// Spawns a new attack line to chill there and let the player know where they are aiming
	/// </summary>
	private void CreateNewLine(Vector3 startPos)
	{
		attackLineStartPos = startPos;
		linePositions = new[] {startPos, startPos};
		attackLine = GameObject.Instantiate(player.AttackLinePrefab, attackLineStartPos, Quaternion.identity).GetComponent<LineRenderer>();
		attackLine.SetPositions(linePositions);
		
		attackLineList.Add(attackLine.gameObject);
	}
}
