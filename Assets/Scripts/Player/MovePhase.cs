﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=============================================//
//---------------MOVEMENT PHASE----------------//
//=============================================//

/// <summary>
/// The state the player is in whenever they are just moving around the map
/// </summary>
public class MovePhase : PlayerPhase
{
    public MovePhase(PlayerController owner)
    {
        player = owner;
    }
	
    //Executes this upon entering the state
    public override void OnEnter()
    {
        player.GetComponentsInChildren<SpriteRenderer>()[1].enabled = false;
    }

    public override void Update()
    {
        //Recharges focus until you have max
        if (player.CurrentFocus < player.AttackCount)
        {
            //Increases recharge rate by 1/4 of itself for each extra pivot over 1
            player.CurrentFocus += Time.deltaTime * (player.FocusRechargeRate + (player.FocusRechargeRate * ((player.AttackCount - 1f)/4f)));
        }

        //If focus is more than at least 1, you can start attacking
        if (InputManager.PressedDown(Inputs.Focus) && Mathf.Floor(player.CurrentFocus) > 0)
        {
            player.SetPhase(PlayerController.Phase.Choosing);
        }
    }

    public override void FixedUpdate()
    {
        if (player.canMove)
            player.Move();
    }

    public override void OnExit()
    {
    }
}

