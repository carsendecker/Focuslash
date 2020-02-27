using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitGateScript : MonoBehaviour
{

    public Animator transitions;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            LoadNextLevel();
        }

    }

    public void LoadNextLevel()
    {
        Debug.Log("Load Next Scene");
        StartCoroutine(LoadLevel(3));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        //Play our animation
        transitions.SetTrigger("start");
        //Wait for it to stop
        yield return new WaitForSeconds(1);
        //Load the scene
        Debug.Log("Load the next scene");
        SceneManager.LoadScene(levelIndex);
    }
}
