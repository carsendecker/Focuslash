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
    [Header("PlayerHealth")]
    public Slider CooldownSlider;
    public Slider PlayerHealthSlider;
    public GameObject PlayerHealthBar;
    public GameObject HeartPrefab;
    public Color FullHeartColor, EmptyHeartColor;
    
    [Header("Focus")]
    public Slider FocusSlider;
    public TMP_Text AttackInstructionText;

    [Header("Score")] 
    public TMP_Text ScoreText;

    [Header("Other Screens")]
    public GameObject LoadingScreen;

    private List<Image> playerHearts = new List<Image>();
    
    
    void Awake()
    {
        if(Services.UI == null)
            Services.UI = this;

        foreach (Transform child in PlayerHealthBar.transform)
        {
//            if (child.Equals(PlayerHealthBar.transform)) return;
            
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
    }

    
    public void UpdatePlayerHealth()
    {
        for (int i = 0; i < playerHearts.Count; i++)
        {
            if (i < Services.Player.GetHealth())
                playerHearts[i].color = FullHeartColor;
            else
                playerHearts[i].color = EmptyHeartColor;

        }

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

    public void UpdatePlayerFocus()
    {
        
    }

}
