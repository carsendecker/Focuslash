using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TargetText : MonoBehaviour
{
    public TMP_Text NumberText;

    private int numberOfTargets;
    private PlayerController player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        numberOfTargets = FindObjectsOfType<TargetText>().Length;
        TargetText[] maybeTarget = transform.parent.GetComponentsInChildren<TargetText>();
        if (maybeTarget.Length > 1)
        {
            maybeTarget[0].NumberText.text += " " + player.EnemyQueue.Count;
            Destroy(gameObject);
        }
        else
        {
            NumberText.text = "";
            NumberText.text += player.EnemyQueue.Count;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
