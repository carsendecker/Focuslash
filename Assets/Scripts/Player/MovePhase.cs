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
//    private float cdTimer;

    public MovePhase(PlayerController owner)
    {
        player = owner;
    }
	
    //Executes this upon entering the state
    public override void OnEnter()
    {
        Services.UI.CooldownSlider.maxValue = player.AttackCount;
        
//        if (player.coolingDown)
//        {
//            cdTimer = 0;
//        }
//        else
//        {
//            cdTimer = player.FocusRechargeRate;
//        }
        Services.UI.CooldownSlider.value = player.CurrentFocus;
        player.GetComponentsInChildren<SpriteRenderer>()[1].enabled = false;

        //Short pause of iFrames after attacking
        player.iFramesForSeconds(0.7f, false);
    }

    public override void Run()
    {
        if(player.canMove)
            player.Move();
		
        //Recharges focus until you have max
        if (player.CurrentFocus < player.AttackCount)
        {
            player.CurrentFocus += Time.deltaTime * player.FocusRechargeRate;
        }

        //If focus is more than at least 1, you can start attacking
        if (InputManager.PressedDown(Inputs.Attack) && Mathf.Floor(player.CurrentFocus) > 0)
        {
            player.SetPhase(PlayerController.Phase.Choosing);
        }
    }

    public override void OnExit()
    {
    }
}

