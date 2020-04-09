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
    [SerializeField] private GameObject PlayerFocusBar;
    [SerializeField] private GameObject FocusBarPrefab;
    public TMP_Text AttackInstructionText;
    [SerializeField] private Color EmptyFocusColor;
    private Color fullFocusColor;

    [Header("Score")] 
    public TMP_Text ScoreText;

    [Header("Other Screens")]
    public GameObject LoadingScreen;

    private List<Image> playerHearts = new List<Image>();
    private List<Slider> playerFocus = new List<Slider>();
    
    
    void Awake()
    {
        Services.UI = this;

        foreach (Transform child in PlayerHealthBar.transform)
        {
            playerHearts.Add(child.GetComponent<Image>());
        }

        foreach (Transform child in PlayerFocusBar.transform)
        {
            playerFocus.Add(child.GetComponent<Slider>());
        }
    }

    private void Start()
    {
        AttackInstructionText.gameObject.SetActive(false);
        
        foreach (var heart in playerHearts)
        {
            heart.color = FullHeartColor;
        }
        
        fullFocusColor = playerFocus[0].GetComponentsInChildren<Image>()[1].color;
        UpdateFocusSliderBounds();
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
        foreach (Slider bar in playerFocus)
        {
            bar.value = Mathf.Lerp(bar.value, Services.Player.CurrentFocus, 0.2f);
        }

        if (playerFocus[0].value < playerFocus[0].maxValue -1)
            playerFocus[0].GetComponentsInChildren<Image>()[1].color = EmptyFocusColor;
        else
            playerFocus[0].GetComponentsInChildren<Image>()[1].color = fullFocusColor;

        //Adds and removes focus bars based on the player's AttackCount
        while (playerFocus.Count != Services.Player.AttackCount)
        {
            //If the UI has less focus than the player's attack count
            if (playerFocus.Count < Services.Player.AttackCount)
            {
                GameObject newBar = Instantiate(FocusBarPrefab, PlayerFocusBar.transform);
                playerFocus.Add(newBar.GetComponent<Slider>());
            }
            //If the UI has more focus than the player's AC
            else if (playerFocus.Count > Services.Player.AttackCount)
            {
                Slider barToRemove = playerFocus[playerFocus.Count - 1];
                playerFocus.Remove(barToRemove);
                Destroy(barToRemove.gameObject);
            }
            
            UpdateFocusSliderBounds();
        }
    }

    /// <summary>
    /// Changes the max and min values of each slider so they work together idk
    /// </summary>
    private void UpdateFocusSliderBounds()
    {
        for (int i = 0; i < playerFocus.Count; i++)
        {
            playerFocus[i].minValue = i;
            playerFocus[i].maxValue = i + 1;
        }
    }

}
