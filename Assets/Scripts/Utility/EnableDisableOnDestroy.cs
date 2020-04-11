using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisableOnDestroy : MonoBehaviour
{
    public GameObject objectToEnable;
    public GameObject objectToDisable;

    void Start()
    {
        if (objectToEnable != null) objectToEnable.SetActive(false);
    }

    private void OnDestroy()
    {
        if (objectToEnable != null) objectToEnable.SetActive(true);
        if (objectToDisable != null) objectToDisable.SetActive(false);

    }
}
