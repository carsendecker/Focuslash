using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpgrade : Creature
{
    public int healthIncreaseNumber;
    public void AddMoreHealth(int healthToAdd)
    {
        Services.Player.MaxHealth += healthToAdd;
        //Services.Player.health += healthToAdd;
    }

    protected override void Die()
    {
        AddMoreHealth(healthIncreaseNumber);
        Services.UI.UpdatePlayerHealth();
        Services.Player.Heal();
        Destroy(this.gameObject);
        Debug.Log(Services.Player.GetHealth());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
        }
    }

}
