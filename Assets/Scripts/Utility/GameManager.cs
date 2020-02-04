using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        if(Services.Game == null)
            Services.Game = this;
    }

    void Update()
    {
        
    }
}
