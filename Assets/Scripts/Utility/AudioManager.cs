using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AMSource
{
    EnemySound = 1,
    PlayerSound = 2,
    Music = 3
}

public class AudioManager : MonoBehaviour
{
//    public const int ENEMY = 0;
    
    public static AudioManager AM;

    private Dictionary<AMSource, AudioSource> sources = new Dictionary<AMSource, AudioSource>();

    private void Awake()
    {
        if (!AM)
        {
            AM = this;
        }

        AudioSource[] asos = GetComponents<AudioSource>();
        sources.Add(AMSource.EnemySound, asos[0]);
        sources.Add(AMSource.PlayerSound, asos[1]);
        sources.Add(AMSource.Music, asos[2]);

    }

    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (sources[AMSource.Music].volume > 0)
            {
                sources[AMSource.Music].volume = 0;
            }
            else
            {
                sources[AMSource.Music].volume = 1;
            }
        }
    }

    public void PlaySound(AudioClip clip, AMSource audioSource)
    {
        sources[audioSource].PlayOneShot(clip);
    }

    public void PlaySound(AudioClip clip, AMSource audioSource, float pitchChange)
    {
        sources[audioSource].pitch += pitchChange;
        sources[audioSource].PlayOneShot(clip);
        sources[audioSource].pitch -= pitchChange;
    }
    
}
