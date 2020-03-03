using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// THIS IS FOR THE PLAYER STATE MACHINE.

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
	public abstract void Update();

	public abstract void FixedUpdate();
	
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