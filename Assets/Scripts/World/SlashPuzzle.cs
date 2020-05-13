using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SlashPuzzle : MonoBehaviour
{
    [HideInInspector] public List<SlashSensor> SlashSensors = new List<SlashSensor>();
    public GameObject DoorToOpen;
    public AudioClip CompleteSound;

    public bool Opened
    {
        get { return isOpened; }
        set
        {
            isOpened = value;
            if (value)
            {
                DoorToOpen.GetComponent<BlockerDoorScript>().makeDoorSlashable();
                Services.Audio.PlaySound(CompleteSound, SourceType.AmbientSound);
                Services.Events.Unregister<PlayerLeftAttackPhase>(ResetSensors);
            }
        }
    }
    private bool isOpened;

    
    
    void Start()
    {
        Services.Events.Register<PlayerLeftAttackPhase>(ResetSensors);
        DoorToOpen.GetComponent<BlockerDoorScript>().closeDoorWay();
    }

    /// <summary>
    /// Gets called when the player stops attacking. Resets all the sensors if they aren't all activated.
    /// Otherwise, open the room.
    /// </summary>
    void ResetSensors(AGPEvent e)
    {
        byte sensorsActive = 0;
        foreach (SlashSensor sensor in SlashSensors)
        {
            if (sensor.Active) sensorsActive++;
        }

        if (sensorsActive == SlashSensors.Count)
        {
            Opened = true;
            return;
        }
        
        foreach (SlashSensor sensor in SlashSensors)
        {
            if(sensor.Active)
                sensor.Active = false;
        }
        
    }
}
