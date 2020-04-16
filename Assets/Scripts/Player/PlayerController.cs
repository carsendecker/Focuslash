using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class PlayerController : Creature
{
	public enum Phase
	{
		Movement = 1,
		Choosing = 2,
		Attacking = 3,
	}
	
	[Tooltip("Number of things player can target at max (aka max 'focus' points).")]
	public int AttackCount;
	
	[Tooltip("How many seconds the player is invincible after being hit.")]
	public float iFrameTime;
	
	[Tooltip("How long (in seconds) a single attack takes to recharge.")]
	public float FocusRechargeRate;

	[Tooltip("How long a single attack line can extend.")]
	public float AttackLineRange;

	[HideInInspector] public float CurrentFocus; //Current focus amount

	[Space(10)]
	public GameObject AttackLinePrefab;
	public GameObject AttackParticlesPrefab;
	public Color SlowMoColor; //The color the camera turns when entering slow motion, will probably get rid of this eventually
	public GameObject LevelUpParticles;
	
	//TODO: Ima fix this somehow, its gross
	public AudioClip hurtSound, attackSound, enterSlomoSound, selectTargetSound, moveTargetSound, deathSound;

	[HideInInspector] public GameObject targetedEnemy;
	[HideInInspector] public bool canMove;
	[HideInInspector] public Queue<Vector2> AttackPositionQueue = new Queue<Vector2>();
	
	private bool invincible;
	private PlayerPhase currentPhase;
	private Dictionary<Phase, PlayerPhase> Phases = new Dictionary<Phase, PlayerPhase>();

	void Awake()
	{
		//Give player to the services manager for easy access
		if (Services.Player == null)
			Services.Player = this;
		
		rb = GetComponent<Rigidbody2D>();
		
		//Adds all the phases to a dictionary for future access
		Phases.Add(Phase.Movement, new MovePhase(this));
		Phases.Add(Phase.Choosing, new ChoosePhase(this));
		Phases.Add(Phase.Attacking, new AttackPhase(this));
	}
	
	new void Start ()
	{
		base.Start();
		
		//Set stats and sliders and such
		canMove = true;
		CurrentFocus = AttackCount;
		
		//Initialize UI bars
		Services.UI.UpdatePlayerHealth();
		
		SetPhase(Phase.Movement);
	}
	

	void Update ()
	{
		currentPhase.Update();
		CurrentFocus = Mathf.Clamp(CurrentFocus, 0, AttackCount);
		Services.UI.UpdatePlayerFocus();
	}

	private void FixedUpdate()
	{
		currentPhase.FixedUpdate();
	}

	//Deals damage to the player
	public override bool TakeDamage(int damage)
	{
		if (invincible) return false;
		
		base.TakeDamage(damage);
		iFramesForSeconds(iFrameTime, true);
		
		Services.UI.UpdatePlayerHealth();
		
		Services.Utility.ShakeCamera(0.5f, 0.3f);
		Services.Audio.PlaySound(hurtSound, SourceType.CreatureSound);
		
		SetPhase(Phase.Movement);

		return false;
	}

	//Deals damage to the player, with an added knockback effect 
	public void TakeDamage(int damage, float knockbackForce, Transform damagingObj)
	{
		if (invincible) return;
		
		TakeDamage(damage);
		
		Vector2 forceDirection = transform.position - damagingObj.position;
		rb.AddForce(forceDirection.normalized * knockbackForce, ForceMode2D.Impulse);
		
	}

	//Kills the player
	protected override void Die()
	{
		EnemySpawner spawner = FindObjectOfType<EnemySpawner>();

		//TODO: Should eventually be replaced by an actual UI controller that gets called from an event or something
		Services.UI.ScoreText.text = "Final Score: " + Services.UI.ScoreText.text;
		Services.UI.ScoreText.transform.localPosition = new Vector2(-100, -150);
		Services.UI.ScoreText.fontSize = 25;
		
		Destroy(spawner);
		Destroy(gameObject);
	}

	//Takes input and moves the player around
	public void Move()
	{
		Vector2 tempVel = rb.velocity;
		
		if (InputManager.Pressed(Inputs.Right))
		{
			tempVel.x = Mathf.Lerp(tempVel.x, MoveSpeed, 0.23f);
		}
		else if (InputManager.Pressed(Inputs.Left))
		{
			tempVel.x = Mathf.Lerp(tempVel.x, -MoveSpeed, 0.23f);
		}
		else
		{
			tempVel.x = Mathf.Lerp(tempVel.x, 0, 0.3f);
		}
		
		if (InputManager.Pressed(Inputs.Up))
		{
			tempVel.y = Mathf.Lerp(tempVel.y, MoveSpeed, 0.23f);
		}
		else if (InputManager.Pressed(Inputs.Down))
		{
			tempVel.y = Mathf.Lerp(tempVel.y, -MoveSpeed, 0.23f);
		}
		else
		{
			tempVel.y = Mathf.Lerp(tempVel.y, 0, 0.3f);
		}
		
		rb.velocity = tempVel;
	}

	/// <summary>
	/// Does a fancy animation and spawns the upgrade items
	/// </summary>
	private void LevelUp()
	{
		//TODO: Actually implement the proper level up item spawns, which needs integration with the level rooms. It now just gives you the stats for testing.
		//IDEA: instead of spawning the items, you are shifted to another "plane" where you can choose, then once you choose you fade back to where you were?
		Instantiate(LevelUpParticles, transform);
		
		Heal();

		AttackCount += 1;
		CurrentFocus = AttackCount;
	}

	
	
	//-----------IFRAME/FLASH FUNCTIONS-----------//
	#region iFrame + Flash Functions
	
	
	/// <summary>
	/// Flashes player sprite
	/// </summary>
	private IEnumerator DamageFlash(float duration)
	{
		SpriteRenderer sr = GetComponent<SpriteRenderer>();
        
		for (int i = 0; i < Math.Round(duration / 0.1f); i++)
		{
			sr.enabled = false;
            
			yield return new WaitForSecondsRealtime(0.06f);
            
			sr.enabled = true;
            
			yield return new WaitForSeconds(0.06f);
		}
	}

	/// <summary>
	/// Gives iFrames for specified length of time, option to flash the player also
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public void iFramesForSeconds(float time, bool flash)
	{
		if (invincible) return;
		if (flash)
			StartCoroutine(DamageFlash(time));
		
		StartCoroutine(iFrames(time));
	}
	
	/// <summary>
	/// Makes player invincible for a specified amount of time, the actual coroutine called by iFramesForSeconds
	/// </summary>
	private IEnumerator iFrames(float time)
	{
		invincible = true;
		yield return new WaitForSecondsRealtime(time);
		invincible = false;
	}
	
	#endregion
	

	//--------TRIGGER/COLLISION FUNCTIONS--------//
	#region Trigger Functions
	
	private void OnCollisionEnter2D(Collision2D other)
	{
		currentPhase.OnCollisionEnter2D(other);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		currentPhase.OnTriggerEnter2D(other);

		//TODO: Should probably change how this works to a normal Vector2 distance value
		if (other.CompareTag("AggroTrigger"))
		{
			other.GetComponentInParent<Enemy>().Aggro(true);
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		currentPhase.OnTriggerExit2D(other);
		
		if (other.CompareTag("AggroTrigger"))
		{
			other.GetComponentInParent<Enemy>().Aggro(false);
		}
	}
	
	//Hurts the player if dangerous particles hit em
	private void OnParticleCollision(GameObject other)
	{
		if (other.CompareTag("Death Particles"))
		{
			//TODO: Passing damage value of particles?
			TakeDamage(1);
		}
	}
	
	#endregion


	//----------STATE MACHINE FUNCTIONS---------//
	#region State Machine Functions

	//Checks to see if the player is in a certain phase
	public bool IsPhase(Phase phaseToCheck)
	{
		if (Phases.ContainsKey(phaseToCheck) && currentPhase.Equals(Phases[phaseToCheck]))
		{
			return true;
		}
		return false;
	}

	//Sets the phase of the player to newPhase
	public void SetPhase(Phase newPhase)
	{
		//Can't enter the same phase its already in
		if (currentPhase != null && IsPhase(newPhase))
		{
//			throw new Exception("Tried to enter the same state again!");
			return;
		}

		currentPhase?.OnExit();
		currentPhase = Phases[newPhase];
		currentPhase.OnEnter();
	}
	
	#endregion

}



