﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Audio can be sent to 3 different audio sources
public enum SourceType
{
    AmbientSound = 1,
    CreatureSound = 2,
    Music = 3
}

/// <summary>
/// A class to handle all audio playing needs, prevents use of having to program audio code + audio sources on a million GameObjects
/// </summary>
public class AudioManager : MonoBehaviour
{   
    //Pairs the AudioSource names with the actual reference
    private Dictionary<SourceType, AudioSource> sources = new Dictionary<SourceType, AudioSource>();

    
    private void Awake()
    {
        if (Services.Audio == null)
            Services.Audio = this;
        
        //Assigns all the audio sources to a sound type
        AudioSource[] asos = GetComponents<AudioSource>();
        sources.Add(SourceType.AmbientSound, asos[0]);
        sources.Add(SourceType.CreatureSound, asos[1]);
        sources.Add(SourceType.Music, asos[2]);
    }

    
    void Update()
    {
        //Pressing M mutes the music just cause
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (sources[SourceType.Music].volume > 0)
                sources[SourceType.Music].volume = 0;
            else
                sources[SourceType.Music].volume = 1;
        }
    }

    /// <summary>
    /// Plays a sound from a specified audio source.
    /// </summary>
    /// <param name="clip">The sound clip to play</param>
    /// <param name="audioSourceType">The audio source type to play from</param>
    public void PlaySound(AudioClip clip, SourceType audioSourceType) => PlaySound(clip, audioSourceType, 0f);

    /// <summary>
    /// Plays a sound from a specified audio source with a certain pitch change applied
    /// </summary>
    public void PlaySound(AudioClip clip, SourceType audioSourceType, float pitchChange)
    {
        sources[audioSourceType].pitch += pitchChange;

        //Set the new music clip
        if (audioSourceType.Equals(SourceType.Music))
        {
            sources[audioSourceType].clip = clip;
            sources[audioSourceType].Play();
        }
        //Play a single sound
        else
        {
            sources[audioSourceType].PlayOneShot(clip);
        }
        
        sources[audioSourceType].pitch -= pitchChange;
    }

    /// <summary>
    /// Stops the music lol
    /// </summary>
    public void StopMusic()
    {
        sources[SourceType.Music].Stop();
    }


    //*****Can add more functions if needed*****

}
