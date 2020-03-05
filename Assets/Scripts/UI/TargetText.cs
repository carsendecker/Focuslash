using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// This class is for the numbers that show up when an enemy is targeted, and indicates the order in which the enemy has been targeted
/// </summary>
public class TargetText : MonoBehaviour
{
    public TMP_Text NumberText;

    private int numberOfTargets;
    private PlayerController player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = Services.Player;
        numberOfTargets = FindObjectsOfType<TargetText>().Length;
        TargetText[] maybeTarget = transform.parent.GetComponentsInChildren<TargetText>();
        if (maybeTarget.Length > 1)
        {
            maybeTarget[0].NumberText.text += " " + player.AttackPositionQueue.Count;
            Destroy(gameObject);
        }
        else
        {
            NumberText.text = "";
            NumberText.text += player.AttackPositionQueue.Count;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
