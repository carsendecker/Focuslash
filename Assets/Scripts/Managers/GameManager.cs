using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<EnemySpawner> LevelRooms = new List<EnemySpawner>();

    private bool endingGame;
    
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
            RestartScene();
        }
    }

    /// <summary>
    /// Called when the player dies, to end the game
    /// </summary>
    public void GameOver()
    {
        if (!endingGame) StartCoroutine(GameOverC());
    }
    
    /// <summary>
    /// Freezes time for a bit, then fades out of the scene before reloading
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameOverC()
    {
        endingGame = true;
        
        Services.Player.enabled = false;
        
        UIManager UI = Services.UI;
        UI.CameraOverlay.color = UI.PlayerDeathColor;
        UI.CameraOverlay.enabled = true;

        Time.timeScale = 0f;
        
        Services.Audio.PlaySound(Services.Player.deathSound, SourceType.CreatureSound);
        Services.Audio.StopMusic();
        
        yield return new WaitForSecondsRealtime(1.5f);
        
        float fadeTime = 0f;
        while (fadeTime < 2.3f)
        {
            Services.MainCamera.orthographicSize -= Time.fixedDeltaTime;
            UI.FullOverlay.color = Color.Lerp(Color.clear, Color.black, fadeTime / 2.3f);
            
            fadeTime += Time.fixedDeltaTime;

            yield return 0;
        }

        endingGame = false;
        RestartScene();

    }

    private void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
