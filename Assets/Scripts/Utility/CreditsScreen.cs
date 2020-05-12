using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreditsScreen : MonoBehaviour
{
    public TMP_Text Title;
    
    void Start()
    {
        Time.timeScale = 1;
        StartCoroutine(Credits());
    }

    private IEnumerator Credits()
    {
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator FadeText(TMP_Text text, bool fadeIn)
    {
        text.alpha = 0;
        text.enabled = true;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            text.alpha = Mathf.Lerp(0, 1, t);
            yield return 0;
        }
    }
}
