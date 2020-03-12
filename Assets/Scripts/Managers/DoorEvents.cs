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

    public event Action onDoorwayTriggerEnter;

    //Still slightly confused on events, but heres what I gathered...
    //DoorwayTriggerEnter can hold on to events that are added from scripts such as DoorScript, which is listening for when an event is triggered...
    public void DoorwayTriggerEnter()
    {
        if (onDoorwayTriggerEnter != null)
        {
            //Invoke the event action
            onDoorwayTriggerEnter();
        }
    }

}
