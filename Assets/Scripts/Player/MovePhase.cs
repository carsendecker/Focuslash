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
    private float cdTimer;

    public MovePhase(PlayerController owner)
    {
        player = owner;
    }
	
    //Executes this upon entering the state
    public override void OnEnter()
    {
        Services.UI.CooldownSlider.maxValue = player.AttackCooldownTime;
        if (player.coolingDown)
        {
            cdTimer = 0;
        }
        else
        {
            cdTimer = player.AttackCooldownTime;
        }
        Services.UI.CooldownSlider.value = cdTimer;
        player.GetComponentsInChildren<SpriteRenderer>()[1].enabled = false;

        //Short pause of iFrames after attacking
        player.iFramesForSeconds(0.5f, false);
    }

    public override void Run()
    {
        if(player.canMove)
            player.Move();
		
        if (cdTimer < player.AttackCooldownTime)
        {
            cdTimer += Time.deltaTime;
            Services.UI.CooldownSlider.value = cdTimer;
			
            if (cdTimer >= player.AttackCooldownTime)
            {
                player.coolingDown = false;
            }
        }

        if (!player.coolingDown && InputManager.PressedDown(Inputs.Attack))
        {
            player.SetPhase(PlayerController.Phase.Choosing);
        }
    }

    public override void OnExit()
    {
    }
}

