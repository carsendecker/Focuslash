using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCheckManager : MonoBehaviour
{
    private int numberOfEnemies;

    void Start()
    {
        numberOfEnemies = 0;
    }

    private void Update()
    {
        Debug.Log(numberOfEnemies);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //We call the event to make it occur here.
        //Doorwaytriggerenter now has the properties of closeDoorWay from the DoorScript

        if (other.gameObject.CompareTag("Player"))
        {
            DoorEvents.current.DoorwayTriggerEnter();
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
