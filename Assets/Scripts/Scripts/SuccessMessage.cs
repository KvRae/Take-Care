using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuccessMessage : MonoBehaviour
{
   public float fadeDuration = 1.5f;

    private CanvasGroup canvasGroup;

    void Start()
    {
        // Get the CanvasGroup component
        canvasGroup = GetComponent<CanvasGroup>();

        // Ensure the message is initially invisible
        canvasGroup.alpha = 0f;
    }

    public void ShowSuccessMessage()
    {
        // Show the message
        StartCoroutine(FadeInAndOut());
    }

    IEnumerator FadeInAndOut()
    {
        // Fade in
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime / fadeDuration;
            yield return null;
        }

        // Wait for a short duration
        yield return new WaitForSeconds(1f);

        // Fade out
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime / fadeDuration;
            yield return null;
        }
    }
}
