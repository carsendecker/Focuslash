using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusUpgrade : Creature
{
    public int FocusIncreaseNumber;
    public void AddMoreFocus(int focusToAdd)
    {
        Services.Player.AttackCount += focusToAdd;
        Services.Player.CurrentFocus = Services.Player.AttackCount;
    }

    protected override void Die()
    {
        AddMoreFocus(FocusIncreaseNumber);
        Services.UI.UpdatePlayerFocus();
        Destroy(this.gameObject);
    }


}
