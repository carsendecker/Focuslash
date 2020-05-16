using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScreen : MonoBehaviour
{
    public TMP_Text Title;
    public TMP_Text[] Names;
    
    void Start()
    {
        Time.timeScale = 1;

        Title.alpha = 0;
        foreach (var name in Names)
        {
            name.gameObject.SetActive(false);
            foreach (TMP_Text child in name.GetComponentsInChildren<TMP_Text>())
            {
                child.alpha = 0;
            }
        }
        StartCoroutine(Credits());
    }

    private IEnumerator Credits()
    {
        StartCoroutine(FadeText(Title, true));
        yield return new WaitForSeconds(4f);
        StartCoroutine(FadeText(Title, false));
        yield return new WaitForSeconds(2f);
        
        foreach (TMP_Text name in Names)
        {
            name.gameObject.SetActive(true);
            foreach (TMP_Text child in name.GetComponentsInChildren<TMP_Text>())
            {
                StartCoroutine(FadeText(child, true));
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(2.5f);
        }
        
        yield return new WaitForSeconds(6.75f);
        
        foreach (TMP_Text name in Names)
        {
            foreach (TMP_Text child in name.GetComponentsInChildren<TMP_Text>())
            {
                StartCoroutine(FadeText(child, false));
            }
        }
        
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(0);
    }

    private IEnumerator FadeText(TMP_Text text, bool fadeIn)
    {
        if (fadeIn) text.alpha = 0;
        else text.alpha = 1;
        
        text.enabled = true;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            if (fadeIn)
            {
                text.alpha = Mathf.Lerp(0, 1, t);
            }
            else
            {
                text.alpha = Mathf.Lerp(1, 0, t);
            }

            yield return 0;
        }
    }
}
