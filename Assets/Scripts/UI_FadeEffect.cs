using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_FadeEffect : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    public void ScreenFade(float targetAlpha, float duration, System.Action onComplete = null)
    {
        StartCoroutine(FadeCoroutine(targetAlpha, duration, onComplete));
    }

    private IEnumerator FadeCoroutine(float targetAlpha, float duration, System.Action onComplete)
    {
        // "System.Action onComplete" this is used for passing a method.
        float time = 0;
        Color currentColor = fadeImage.color;

        // "a" is the parameter that we are going to change in the color.
        float startAlpha = currentColor.a;

        while (time < duration)
        {
            time += Time.deltaTime;

            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);

            fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            // This is wait for the new frame to work, "frame by frame"
            yield return null; 
        }

        fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);

        onComplete?.Invoke();
    }
}
