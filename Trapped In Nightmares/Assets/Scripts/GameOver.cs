using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Image fadeImage;
    public Image fadeImage2;

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(FadeImage2(false));
    }

    private IEnumerator FadeImage(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1.3f; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                fadeImage.color = new Color(0, 0, 0, i);
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= 2.5f; i += Time.deltaTime)
            {
                // set color with i as alpha
                fadeImage.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
        
        yield return new WaitForSeconds(3f);
        Application.Quit();
    }
    
    private IEnumerator FadeImage2(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1.3f; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                fadeImage2.color = new Color(0, 0, 0, i);
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= 2f; i += Time.deltaTime)
            {
                // set color with i as alpha
                fadeImage2.color = new Color(0, 0, 0, i);
                yield return null;
            }
        }

        StartCoroutine(FadeImage(false));

    }
}
