using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class handles a lot of random functions regarding the overall game state/visuals
/// </summary>
public class UtilityManager : MonoBehaviour
{
    
    private Camera mainCam;
    private CameraShake cShake;
    private bool gameFrozen;

    void Awake()
    {
        if (Services.Utility == null)
            Services.Utility = this;
    }
    

    void Start()
    {
        mainCam = Camera.main;
        cShake = mainCam.GetComponent<CameraShake>();
    }

    void Update()
    {
        if (InputManager.PressedDown(Inputs.Restart))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }

    /// <summary>
    /// Shakes the camera for duration
    /// </summary>
    public void ShakeCamera(float duration)
    {
        cShake.ShakeCamera(duration);
    }

    /// <summary>
    /// Shakes the camera for a duration at a level of magnitude
    /// </summary>
    public void ShakeCamera(float duration, float magnitude)
    {
        float prev = cShake.shakeMagnitude;
        cShake.shakeMagnitude = magnitude;
        
        cShake.ShakeCamera(duration);
        
//        cShake.shakeMagnitude = prev;
    }

    /// <summary>
    /// Fades a color's alpha to 0 at a certain speed (I guess this is usually supposed to be called in some form of update loop?)
    /// </summary>
    public bool FadeOut(Color colorToFade, float speed)
    {
        colorToFade.a -= Mathf.Clamp(speed, 0, 1);
        if (colorToFade.a <= 0)
        {
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Fades a color's alpha to 1 at a certain speed (I guess this is usually supposed to be called in some form of update loop?)
    /// </summary>
    public bool FadeIn(Color colorToFade, float speed)
    {
        colorToFade.a += Mathf.Clamp(speed, 0, 1);
        if (colorToFade.a >= 1)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Makes the game freeze for a certain amount of time.
    /// </summary>
    public void FreezeForSeconds(float seconds)
    {
        if (gameFrozen) return;
        StartCoroutine(FreezeForSecondsC(seconds));
    }
    
    private IEnumerator FreezeForSecondsC(float seconds, float scale = 0)
    {
        float prev = Time.timeScale;

        Time.timeScale = scale;
        gameFrozen = true;
        
        yield return new WaitForSecondsRealtime(seconds);
        
        gameFrozen = false;
        Time.timeScale = prev;
    }
}
