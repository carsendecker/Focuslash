using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Creature
{
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
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
