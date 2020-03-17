using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCheckManager : MonoBehaviour
{
    private int numberOfEnemies;

    void Start()
    {
        
    }

    private void Update()
    {
        Debug.Log(numberOfEnemies);
        if (numberOfEnemies == 0)
        {
            //Call the event 
            DoorEvents.current.EnemiesDefeated();
            Debug.Log("The Door is Open!");
        }
    }

  
    private void OnTriggerEnter2D(Collider2D other)
    {
        //We call the event to make it occur here.
        //Doorwaytriggerenter now has the properties of closeDoorWay from the DoorScript
        if (other.gameObject.CompareTag("Player"))
        {
            DoorEvents.current.DoorwayTriggerEnter();
            Debug.Log("The door is closed.");
        }
       
        
        if (other.gameObject.CompareTag("Enemy") )
        {
            numberOfEnemies++;
        }

       
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            numberOfEnemies--;
        }
    }
}
