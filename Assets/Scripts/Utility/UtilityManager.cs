using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UtilityManager : MonoBehaviour
{
    
    private Camera mainCam;
    private CameraShake cShake;
    private bool gameFrozen;

    void Awake()
    {
        if (SystemsManager.Utility == null)
            SystemsManager.Utility = this;
    }
    

    void Start()
    {
        mainCam = Camera.main;
        cShake = mainCam.GetComponent<CameraShake>();
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.PressedDown(Inputs.Restart))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }

    //Shakes the camera for duration
    public void ShakeCamera(float duration)
    {
        cShake.ShakeCamera(duration);
    }

    //Shakes the camera for duration at magnitude
    public void ShakeCamera(float duration, float magnitude)
    {
        float prev = cShake.shakeMagnitude;
        cShake.shakeMagnitude = magnitude;
        
        cShake.ShakeCamera(duration);
        
//        cShake.shakeMagnitude = prev;
    }

    public bool FadeOut(Color colorToFade, float speed)
    {
        colorToFade.a -= Mathf.Clamp(speed, 0, 1);
        if (colorToFade.a <= 0)
        {
            return true;
        }
        return false;
    }
    
    public bool FadeIn(Color colorToFade, float speed)
    {
        colorToFade.a += Mathf.Clamp(speed, 0, 1);
        if (colorToFade.a >= 1)
        {
            return true;
        }
        return false;
    }

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
