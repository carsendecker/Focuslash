using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashSensor : MonoBehaviour
{
    public Color InactiveColor, ActiveColor;
    [Range(0, 0.5f)] public float ActiveTime;
    public bool Active;
    public AudioClip stepSound;

    private SlashPuzzle parentPuzzle;
    private SpriteRenderer sr;
    private float fadingColor;
    private float activeTimer;
    
    
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        parentPuzzle = GetComponentInParent<SlashPuzzle>();
        parentPuzzle.SlashSensors.Add(this);
        activeTimer = 999f;
    }

    private void Update()
    {
        if (activeTimer <= ActiveTime)
        {
            sr.color = Color.Lerp(ActiveColor, InactiveColor, activeTimer / ActiveTime);
            activeTimer += Time.deltaTime;
        }
        else Active = false;
    }

    private void Activate()
    {
        Active = true;
        parentPuzzle.SensorActivated();
        activeTimer = 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Services.Audio.PlaySound(stepSound, SourceType.CreatureSound);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Activate();
        }
    }

    public void Complete()
    {
        sr.color = ActiveColor;
        Destroy(this);
    }
}
