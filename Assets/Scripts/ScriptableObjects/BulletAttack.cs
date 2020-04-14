using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[CreateAssetMenu(fileName = "New_BulletAttack", menuName = "Creatures/BulletAttack")]
public class BulletAttack : Attack
{
	public enum FireDirection
	{
		Forward, Backward, Left, Right
	}

	[System.Serializable]
	public struct BulletPattern
	{
		[Tooltip("Direction to fire the bullet.")]
		public FireDirection Direction;
		
		[Tooltip("Bullets fired per second")]
		public int fireRate;
		
		[Tooltip("Angle at which to fire the bullet, zero = center.")]
		[Range(-90, 90)]
		public float Angle;
		
		[Tooltip("Offsets the starting position of the bullet from the emitter, zero = center. Axis of offset varies by FireDirection.")]
		public float Offset;
	}

	public GameObject BulletPrefab;

	[Tooltip("List of bullet patterns that will be fired as part of the attack. Each pattern is one emission point for bullets.")]
	public List<BulletPattern> Patterns;
}