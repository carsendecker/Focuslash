using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArena : MonoBehaviour
{
    public float IntroDuration;
    public GameObject Boss;
    public AudioClip BossMusic;

    private bool cutsceneStarted;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !cutsceneStarted)
        {
            StartCoroutine(IntroCutscene());
        }
    }

    /// <summary>
    /// Plays a nice intro
    /// </summary>
    private IEnumerator IntroCutscene()
    {
        cutsceneStarted = true;
        
        Services.Player.canMove = false;
        Services.Player.rb.velocity = Vector2.zero;
        
        Services.Audio.StopMusic();
        
        yield return new WaitForSeconds(1f);
        
        CameraFollow camFollow = Services.MainCamera.GetComponentInParent<CameraFollow>();

        float originalScale = camFollow.lerpScale;
        camFollow.lerpScale = 1.5f;
        camFollow.objToFollow = Boss.transform;
        
        yield return new WaitForSeconds(IntroDuration / 2);
        
        Services.Audio.PlaySound(BossMusic, SourceType.Music);
        Services.UI.EnableBossHealth(IntroDuration / 2.3f);
        
        yield return new WaitForSeconds(IntroDuration / 2);
        
        camFollow.lerpScale = originalScale;
        camFollow.objToFollow = Services.Player.transform;
        Services.Player.canMove = true;

        Boss.GetComponent<BossScript>().Aggro(true);

    }
}
