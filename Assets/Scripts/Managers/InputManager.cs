using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Inputs
{
    None=0,
    Up=1,
    Right=2,
    Down=3,
    Left=4,
    Attack=5,
    Target=6,
    Restart=7,
    Cancel=8
}

public static class InputManager 
{
    public static bool Pressed(Inputs i)
    {
        switch (i)
        {
            case Inputs.Up:
                return Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
            case Inputs.Left:
                return Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
            case Inputs.Right:
                return Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
            case Inputs.Down:
                return Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
            case Inputs.Attack:
                return Input.GetKey(KeyCode.Space);
            case Inputs.Target:
                return Input.GetKey(KeyCode.Mouse0);
            case Inputs.Restart:
                return Input.GetKey(KeyCode.R);
        }

        return false;
    }
    
    public static bool PressedDown(Inputs i)
    {
        switch (i)
        {
            case Inputs.Up:
                return Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
            case Inputs.Left:
                return Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
            case Inputs.Right:
                return Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
            case Inputs.Down:
                return Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
            case Inputs.Attack:
                return Input.GetKeyDown(KeyCode.Space);
            case Inputs.Target:
                return Input.GetKeyDown(KeyCode.Mouse0);
            case Inputs.Restart:
                return Input.GetKeyDown(KeyCode.R);
            case Inputs.Cancel:
                return Input.GetKeyDown(KeyCode.Q);
        }

        return false;
    }
    
    public static bool PressedUp(Inputs i)
    {
        switch (i)
        {
            case Inputs.Up:
                return Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W);
            case Inputs.Left:
                return Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A);
            case Inputs.Right:
                return Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D);
            case Inputs.Down:
                return Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S);
            case Inputs.Attack:
                return Input.GetKeyUp(KeyCode.Space);
            case Inputs.Target:
                return Input.GetKeyUp(KeyCode.Mouse0);
            case Inputs.Restart:
                return Input.GetKeyUp(KeyCode.R);
        }

        return false;
    }
    
}

