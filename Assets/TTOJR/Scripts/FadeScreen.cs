using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeScreen : MonoBehaviour
{
    public bool isFading = false;
    public Image panel;
    public float incrementDelay = 0.03f;
    public float fadeStep = 0.02f;

    public void FadeToBlack()
    {
        StartCoroutine(C_FadeToBlack());
    }

    public void FadeToVisible()
    {
        StartCoroutine(C_FadeToVisible());
    }

    public void FadeInAndOut()
    {
        StartCoroutine(C_FadeInAndOut());
    }
    public void FadeInAndOutCallback(Action callbackOnFadeOutComplete = null, Action callbackCompleted = null, float blackScreenTime = 0f)
    {
        StartCoroutine(C_FadeInAndOut(callbackOnFadeOutComplete, callbackCompleted, blackScreenTime));
    }

    IEnumerator C_FadeInAndOut(Action callbackOnFadeOutComplete = null, Action callbackCompleted = null, float blackScreenTime = 0f)
    {
        if (isFading) yield break;
        yield return StartCoroutine(C_FadeToBlack());
        float fadeDuration = (1f / fadeStep) * incrementDelay;
        yield return new WaitForSeconds(fadeDuration * 0.5f);
        callbackOnFadeOutComplete?.Invoke();

        if(blackScreenTime > 0f) yield return new WaitForSeconds(blackScreenTime);

        yield return StartCoroutine(C_FadeToVisible(callbackCompleted));
    }



    IEnumerator C_FadeToBlack()
    {
        isFading = true;
        Color fade = Color.black;
        fade.a = 0;
        float increment = 0;
        while (increment < 0.95f)
        {
            increment += 0.02f;
            yield return new WaitForSeconds(incrementDelay);
            fade.a = increment;
            panel.color = fade;
        }
        fade.a = 1;
        panel.color = fade;
        isFading = false;
    }

    IEnumerator C_FadeToVisible(Action callback = null)
    {
        isFading = true;
        Color fade = Color.black;
        fade.a = 1;
        float increment = 1;
        while (increment > 0.05f)
        {
            increment -= 0.02f;
            yield return new WaitForSeconds(incrementDelay);
            fade.a = increment;
            panel.color = fade;
        }
        fade.a = 0;
        panel.color = fade;
        isFading = false;
        callback?.Invoke();
    }
}