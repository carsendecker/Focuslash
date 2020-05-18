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
	
	//TODO: Make it so that you do not dash if your target location is over a pit!!!
	
	private GameObject targetedEnemy;
	private GameObject crosshair;
	private LineRenderer attackLine;
	private Vector3 attackLineStartPos;
	private float startingFocus; //How much focus you start choosing with, used for when you cancel your attack
	
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
		Time.timeScale = 0.1f;
		Services.Audio.PlaySound(player.enterSlomoSound, SourceType.CreatureSound);
		Services.UI.CameraOverlay.enabled = true;
		Services.UI.CameraOverlay.color = Services.UI.PlayerFocusColor;

		
		startingFocus = player.CurrentFocus;

		player.GetComponent<Rigidbody2D>().velocity /= 50;

		//Turns on range indicator sprite
		player.GetComponentsInChildren<SpriteRenderer>()[1].enabled = true;
		Services.UI.AttackInstructionText.gameObject.SetActive(true);


		// if (Mathf.Floor(startingFocus) < 1)
		// {
		// 	
		// }
		CreateNewLine(player.transform.position);
		player.CurrentFocus--;
	}

	//Runs every update frame
	public override void Update()
	{
		if (InputManager.PressedUp(Inputs.Focus))
		{
			SetTarget();

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

		// TargetWithMouse();
		DrawAttackLine();

		if (InputManager.PressedDown(Inputs.Target) && player.CurrentFocus >= 1)
		{
			//Adds the targeted enemy to a "queue" for later
			SetTarget();
			player.CurrentFocus--;

			Services.Audio.PlaySound(player.selectTargetSound, SourceType.CreatureSound);
			
			CreateNewLine(attackLine.GetPosition(1));
		}

	}
	
	public override void FixedUpdate()
	{
	}

	public override void OnExit()
	{
		Services.UI.AttackInstructionText.gameObject.SetActive(false);
		Services.UI.CameraOverlay.enabled = false;

		Time.timeScale = 1f;

		if (crosshair != null)
		{
			GameObject.Destroy(crosshair);
			crosshair = null;
		}

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

	/// <summary>
	/// Enqueues the position at the end of the drawn line, and reduce focus
	/// </summary>
	private void SetTarget()
	{
		player.AttackPositionQueue.Enqueue(attackLine.GetPosition(1));
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
