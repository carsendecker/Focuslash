using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[CreateAssetMenu(fileName = "BulletAttack", menuName = "Creatures/BulletAttack")]
public class BulletAttack : ScriptableObject
{
    // [System.Serializable]
    // public struct Direction
    // {
    //     public bool up, down, left, right, upLeft, upRight, downLeft, downRight;
    //     [Space(10)]
    //     public bool all;
    //     public bool atPlayer;
    // }
    
    public enum FireDirection

    [System.Serializable]
    public struct BulletPattern
    {
        
    }

    public GameObject BulletPrefab;
    public int damagePerBullet;
    
    [Tooltip("Bullets fired per second, per emitter")]
    public int fireRate;

    [Tooltip("The directions that bullets will shoot in at once.")]
    public Directions directionsToShoot;
}
