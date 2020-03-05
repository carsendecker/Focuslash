using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public GameObject focusUpgrade;
    public GameObject healthUpgrade;
    void Start()
    {
        
    }

   
    void Update()
    {
        if (focusUpgrade == null)
        {
            healthUpgrade.SetActive(false);
        }

        if (healthUpgrade == null)
        {
            focusUpgrade.SetActive(false);
        }
    }
}
