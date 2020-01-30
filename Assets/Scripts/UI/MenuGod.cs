using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuGod : MonoBehaviour
{
    public static MenuGod MG;
    [Header("Attack UI")]
    public GameObject AttackPanel;
    public List<TMP_Text> AttackOptions = new List<TMP_Text>();

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
    
    // Start is called before the first frame update
    void Awake()
    {
        if(MG == null)
            MG = this;
    }

    private void Start()
    {
        TimelineSlider.gameObject.SetActive(false);
        TimelineInstructionText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

//    public void ChangeSliderValue(Slider slider, float value)
//    {
//        
//    }
//
//    public float LerpSlider(Slider slider, float lerpTo, float speed)
//    {
//        return Mathf.Lerp(slider.value, lerpTo, speed);
//    }
}
