using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f; 
    public void StartTheGame()
    {

        StartCoroutine(LoadLevel("NewTutorial"));
    }

    IEnumerator LoadLevel(string sceneName)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }
    
    
}