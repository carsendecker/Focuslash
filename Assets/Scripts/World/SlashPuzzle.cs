using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SlashPuzzle : MonoBehaviour
{
    [HideInInspector] public List<SlashSensor> SlashSensors = new List<SlashSensor>();
    public GameObject DoorToOpen;

    public bool Opened
    {
        get { return isOpened; }
        set
        {
            isOpened = value;
            if (value)
            {
                Destroy(DoorToOpen.gameObject);
                Services.Events.Unregister<PlayerLeftAttackPhase>(ResetSensors);
            }
        }
    }
    private bool isOpened;

    
    
    void Start()
    {
        Services.Events.Register<PlayerLeftAttackPhase>(ResetSensors);
    }

    void ResetSensors(AGPEvent e)
    {
        byte sensorsActive = 0;
        foreach (SlashSensor sensor in SlashSensors)
        {
            if (sensor.Active) sensorsActive++;
        }

        if (sensorsActive >= SlashSensors.Count)
            Opened = true;
        
        if (!Opened)
        {
            foreach (SlashSensor sensor in SlashSensors)
            {
                sensor.Active = false;
            }
        }
        
    }
}
