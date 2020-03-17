using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorEvents : MonoBehaviour
{
    
    //Reference to the current script
    public static DoorEvents current;

    public void Awake()
    {
        current = this;
    }

    //I GOT IT
    //This event action holds on to functions.
    public event Action onDoorwayTriggerEnter;

  //However, right now, the function doesnt have anything within it.
  //Other scripts need to subscribe its function into it, a sort of function within a function.
  //Then it can be invoked in whatever means needed by doing DoorEvents.Current.(name of the function thats in the script)
    public void DoorwayTriggerEnter()
    {
        if (onDoorwayTriggerEnter != null)
        {
            //Invoke the event action
            onDoorwayTriggerEnter();
        }
    }
    
    //A new event to add on. 
    public event Action onEnemiesDefeated;

    public void EnemiesDefeated()
    {
        if (onEnemiesDefeated != null)
        {
            //Invoke the event's action which is to be added from another script.
            onEnemiesDefeated();
        }
    }

}
