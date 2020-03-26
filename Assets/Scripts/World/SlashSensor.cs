using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashSensor : Creature
{
    public bool Active
    {
        get { return isActive; }
        set
        {
            isActive = value;
            if (value)
                sr.color = ActiveColor;
            else
                fadingColor = true;

        }
    }

    public Color InactiveColor, ActiveColor;

    private SlashPuzzle parentPuzzle;
    private bool isActive;
    private SpriteRenderer sr;
    private bool fadingColor;
    
    
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        parentPuzzle = GetComponentInParent<SlashPuzzle>();
        parentPuzzle.SlashSensors.Add(this);
        
    }

    private void Update()
    {
        if (fadingColor)
        {
            sr.color = Color.Lerp(sr.color, InactiveColor, 0.05f);

            if (sr.color.Equals(InactiveColor))
                fadingColor = false;
        }
    }


    public override bool TakeDamage(int damage)
    {
        Active = true;
        return false;
    }
}
