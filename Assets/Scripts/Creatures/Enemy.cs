using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Creature
{
    private EnemySpawner currentRoom;
    
    protected override void Start()
    {
        base.Start();
        currentRoom = GetComponentInParent<EnemySpawner>();
    }

    protected virtual void Update()
    {
        
    }
    
    /// <summary>
    /// For making a creature aggressive towards another.
    /// 
    /// Call this from the creature that *will be aggro'd onto*
    /// (ie call this from the player when entering enemy range [enemy.Aggro(true) or something])
    /// </summary>
    public virtual void Aggro(bool aggrod)
    {
        this.enabled = aggrod;
    }

    protected override void Die()
    {
    }
}
