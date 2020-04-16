using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that stores references to most UI elements for easy access
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Player Health")]
    [SerializeField] private GameObject PlayerHealthBar;
    [SerializeField] private GameObject HeartPrefab;
    [SerializeField] private Color FullHeartColor, EmptyHeartColor;

    [Header("Player Focus")]
//    public Slider FocusSlider;
    [SerializeField] private Slider PlayerFocusBar;
    [SerializeField] private Sprite[] FocusBarSprites = new Sprite[3];
    public TMP_Text AttackInstructionText;
    [SerializeField] private Color EmptyFocusColor;
    private Color fullFocusColor;

    [Header("Score")] 
    public TMP_Text ScoreText;

    [Header("Other Screens")]
    public GameObject LoadingScreen;

    private List<Image> playerHearts = new List<Image>();

    // private List<Slider> playerFocus = new List<Slider>();
    
    
    void Awake()
    {
        Services.UI = this;

        foreach (Transform child in PlayerHealthBar.transform)
        {
            playerHearts.Add(child.GetComponent<Image>());
        }

        // foreach (Transform child in PlayerFocusBar.transform)
        // {
        //     playerFocus.Add(child.GetComponent<Slider>());
        // }
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

}
