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

    private List<Color> fadingColors = new List<Color>();

    void Awake()
    {
        Services.Utility = this;
    }
    
    void Start()
    {
        mainCam = Camera.main;
        cShake = mainCam.GetComponent<CameraShake>();
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
    /// BROKEN LOL Fades a color's alpha to 0 at a certain speed (I guess this is usually supposed to be called in some form of update loop?)
    /// </summary>
    public void FadeOut(Color colorToFade, float speed)
    {
        if (fadingColors.Contains(colorToFade)) return;

        StartCoroutine(FadeOutC(colorToFade, speed));
    }

    /// <summary>
    /// The actual coroutine to handle the fading independently, should not called anywhere but here
    /// </summary>
    private IEnumerator FadeOutC(Color colorToFade, float speed)
    {
        Color changingColor = colorToFade;
        fadingColors.Add(colorToFade);
        
        while (colorToFade.a > 0)
        {
            colorToFade.a -= Mathf.Clamp(speed, 0, 1);
            yield return 0;
        }

        fadingColors.Remove(colorToFade);

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

    // private List<bool> currentWaitingBools = new List<bool>();
    // public void WaitForSecondsThenTrue(float seconds, ref bool boolToSet)
    // {
    //     if (currentWaitingBools.Contains(boolToSet))
    //     {
    //         if (currentWaitingBools[currentWaitingBools.IndexOf(boolToSet)] == true)
    //         {
    //             boolToSet = true;
    //             currentWaitingBools.Remove(boolToSet);
    //             return;
    //         }
    //     }
    //     
    //     currentWaitingBools.Add(boolToSet);
    //     StartCoroutine(WaitPauseC(seconds, boolToSet));
    //     
    // }
    //
    // private IEnumerator WaitPauseC(float seconds, bool boolToChange)
    // {
    //     yield return new WaitForSeconds(seconds);
    //     currentWaitingBools[currentWaitingBools.IndexOf(boolToChange)] = true;
    // }
}
