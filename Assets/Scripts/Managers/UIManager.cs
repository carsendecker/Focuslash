using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

/// <summary>
/// Class that stores references to most UI elements for easy access
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Player Health")]
    [SerializeField] private GameObject PlayerHealthBar = null;
    [SerializeField] private GameObject HeartPrefab = null;
    [SerializeField] private Color FullHeartColor, EmptyHeartColor;

    [Header("Player Focus")]
//    public Slider FocusSlider;
    [SerializeField] private Slider PlayerFocusBar = null;
    [SerializeField] private Sprite[] FocusBarSprites = new Sprite[3];
    public TMP_Text AttackInstructionText = null;
    [SerializeField] private Color EmptyFocusColor;
    private Color fullFocusColor;

    [Header("Overlays")] 
    public SpriteRenderer CameraOverlay;
    public Color PlayerFocusColor, PlayerDeathColor;
    public Image FullOverlay;
    public PostProcessVolume PostProcess;

    [Header("Boss")] 
    [SerializeField] private bool SceneHasBoss;
    [SerializeField] private Slider BossHealthBar;
    [SerializeField] private Creature BossEnemy;
    private bool bossIntroDone;
    private float bossLerpTime;

    [Header("Other Screens")]
    public GameObject LoadingScreen;

    private List<Image> playerHearts = new List<Image>();
    
    
    void Awake()
    {
        Services.UI = this;

        foreach (Transform child in PlayerHealthBar.transform)
        {
            playerHearts.Add(child.GetComponent<Image>());
        }
    }

    private void Start()
    {
        AttackInstructionText.gameObject.SetActive(false);
        
        foreach (var heart in playerHearts)
        {
            heart.color = FullHeartColor;
        }
        
        fullFocusColor = PlayerFocusBar.GetComponentsInChildren<Image>()[1].color;
        PlayerFocusBar.maxValue = 3;
        CameraOverlay.enabled = false;
    }

    private void Update()
    {
        if (bossLerpTime > 0)
        {
            BHBTransform.sizeDelta = Vector2.Lerp(BHBTransform.sizeDelta, new Vector2(BHBPrevWidth, BHBTransform.sizeDelta.y), 0.1f);
            BossHealthBar.GetComponentInChildren<TMP_Text>().enabled = true;
            bossLerpTime -= Time.deltaTime;

            if (bossLerpTime <= 0)
            {
                bossIntroDone = true;
            }
        }
        
        if (SceneHasBoss && bossIntroDone)
        {
            UpdateBossHealth();
        }
    }

    /// <summary>
    /// Updates the UI for the player's health bar
    /// </summary>
    public void UpdatePlayerHealth()
    {
        //Changes heart colors based on current health
        for (int i = 0; i < playerHearts.Count; i++)
        {
            if (i < Services.Player.GetHealth())
                playerHearts[i].color = FullHeartColor;
            else
                playerHearts[i].color = EmptyHeartColor;

        }

        //Adds and removes hearts based on the player's MaxHealth
        while (playerHearts.Count != Services.Player.MaxHealth)
        {
            //If the UI has less hearts than the player's max health
            if (playerHearts.Count < Services.Player.MaxHealth)
            {
                GameObject newHeart = Instantiate(HeartPrefab, PlayerHealthBar.transform);
                playerHearts.Add(newHeart.GetComponent<Image>());
            }
            //If the UI has more hearts
            else if (playerHearts.Count > Services.Player.MaxHealth)
            {
                Image heartToRemove = playerHearts[playerHearts.Count - 1];
                playerHearts.Remove(heartToRemove);
                Destroy(heartToRemove.gameObject);
            }
        }
    }

    /// <summary>
    /// Updates the UI bars for the player's current focus
    /// </summary>
    public void UpdatePlayerFocus()
    {
        //Updates values of each bar
        PlayerFocusBar.value = Mathf.Lerp(PlayerFocusBar.value, Services.Player.CurrentFocus, 0.32f);
        Image[] sliderSprites = PlayerFocusBar.GetComponentsInChildren<Image>();
        
        //Changes the color if full
        if (PlayerFocusBar.value < 0.98f)
            sliderSprites[1].color = EmptyFocusColor;
        else 
            sliderSprites[1].color = fullFocusColor;
        
        foreach (Image image in sliderSprites)
        {
            image.sprite = FocusBarSprites[Services.Player.AttackCount - 1];
        }

    }

    
    private float BHBPrevWidth;
    private RectTransform BHBTransform;
    /// <summary>
    /// Enables and expands the boss' health bar all cool-like.
    /// </summary>
    /// <param name="introTime"> The length of time that it will take to fully lerp in the bar. </param>
    public void EnableBossHealth(float introTime)
    {
        if (bossLerpTime > 0) return;

        BossHealthBar.GetComponentInChildren<TMP_Text>().enabled = false;

        BHBTransform = BossHealthBar.GetComponent<RectTransform>();
        BHBPrevWidth = BHBTransform.sizeDelta.x;
        BHBTransform.sizeDelta = new Vector2(0, 15f);

        BossHealthBar.maxValue = BossEnemy.MaxHealth;
        BossHealthBar.value = BossHealthBar.maxValue;
        BossHealthBar.gameObject.SetActive(true);
        
        bossLerpTime = introTime;
    }
    

    private void UpdateBossHealth()
    {
        BossHealthBar.value = Mathf.Lerp(BossHealthBar.value, BossEnemy.GetHealth(), 0.2f);
    }

}
