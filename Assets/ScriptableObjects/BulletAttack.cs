using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
	Up, Down, Left, Right
}

[CreateAssetMenu(fileName = "New_BulletAttack", menuName = "Creatures/BulletAttack")]
public class BulletAttack : Attack
{
	

	[System.Serializable]
	public struct BulletPattern
	{
		[Tooltip("Direction to fire the bullet.")]
		public Direction Direction;
		
		[Tooltip("Bullets fired per second.")]
		public float FireRate;

		[Tooltip("Speed of bullets fired.")]
		public float BulletSpeed;
		
		[Tooltip("Angle at which to fire the bullet, zero = center.")]
		[Range(-90, 90)]
		public float Angle;

		[Tooltip("Offsets the starting position of the bullet from the emitter, zero = center. Axis of offset varies by FireDirection.")]
		public float Offset;
	}

	public GameObject BulletPrefab;

	public AudioClip FireSound;
	
	[Tooltip("Time (in seconds) that the attack lasts.")]
	public float AttackDuration;

	[Tooltip("Rotate the emitters around the enemy at a certain speed.")]
	public float RotationSpeed;

	[Tooltip("List of bullet patterns that will be fired as part of the attack. Each pattern is one emission point for bullets.")]
	public List<BulletPattern> Patterns;
}