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
    

    void Start()
    {
        DoorToOpen.GetComponent<BlockerDoorScript>().closeDoorWay();
    }

    public void SensorActivated()
    {
        if (!Services.Player.IsPhase(PlayerController.Phase.Attacking)) return;
        
        int sensorsActive = 0;
        foreach (SlashSensor sensor in SlashSensors)
        {
            if (sensor.Active) sensorsActive++;
        }

        if (sensorsActive == SlashSensors.Count)
        {
            Open();
        }
    }

    private void Open()
    {
        foreach (SlashSensor sensor in SlashSensors)
        {
            sensor.Complete();
        }
        
        DoorToOpen.GetComponent<BlockerDoorScript>().makeDoorSlashable();
        Services.Audio.PlaySound(CompleteSound, SourceType.AmbientSound);
    }
}
