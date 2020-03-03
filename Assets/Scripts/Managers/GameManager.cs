using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        if(Services.Game == null)
            Services.Game = this;
        
        Services.InitializeServices();
    }

    void Update()
    {
        if (InputManager.PressedDown(Inputs.Restart))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
