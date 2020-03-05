using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Creature
{
    [Tooltip("How much XP this enemy is worth to the player.")]
    public float XPValue;
    
    // Start is called before the first frame update
    void Start()
    {
        
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
        Services.Player.GainXP(XPValue);
    }
}
