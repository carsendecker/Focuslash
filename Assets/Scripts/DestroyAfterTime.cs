using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float TimeUntilDestruction;
    private float timer;
    
    // Start is called before the first frame update
    void Start()
    {
        timer = TimeUntilDestruction;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
