using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpgrade : MonoBehaviour
{
    public int healthIncreaseNumber;
    public void AddMoreHealth(int healthToAdd)
    {
        Services.Player.MaxHealth += healthToAdd;
        //Services.Player.health += healthToAdd;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AddMoreHealth(healthIncreaseNumber);
            Services.UI.UpdatePlayerHealth();
            
            Destroy(this.gameObject);
            Debug.Log(Services.Player.GetHealth()); 
        }
    }

}
