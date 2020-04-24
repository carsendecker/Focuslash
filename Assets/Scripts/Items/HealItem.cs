using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItem : Creature
{
    public int heartsToRestore;
    public ParticleSystem DeathParticles;

    protected override void Die()
    {
        Instantiate(DeathParticles, transform.position, Quaternion.identity);
        Services.Player.Heal(heartsToRestore);
        Destroy(this.gameObject);
    }

}
