using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that stores references to most UI elements for easy access
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Bars")]
    public Slider CooldownSlider;
    public Slider PlayerHealthSlider;
    
    [Header("Timeline")]
    public Slider TimelineSlider;
    public TMP_Text TimelineInstructionText;

    [Header("Score")] 
    public TMP_Text ScoreText;

    [Header("Other Screens")]
    public GameObject LoadingScreen;
    
    
    
    void Awake()
    {
        if(Services.UI == null)
            Services.UI = this;
    }

    private void Start()
    {
        TimelineSlider.gameObject.SetActive(false);
        TimelineInstructionText.gameObject.SetActive(false);
    }


}
