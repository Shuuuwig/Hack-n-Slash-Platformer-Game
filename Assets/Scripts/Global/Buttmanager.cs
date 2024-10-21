using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Buttmanager: MonoBehaviour
{
    private CanvasGroup canvasGroup;

    public float fadeDuration = 1.5f; 
    public float delayBeforeFade = 1.5f; 

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0; 
            StartCoroutine(FadeIn());
        }
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(delayBeforeFade);

        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1; 
    }
}
