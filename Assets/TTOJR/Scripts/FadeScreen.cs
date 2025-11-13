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

    public void FadeToBlack(Action? posthook = null)
    {
        StartCoroutine(C_FadeToBlack(posthook));
    }

    public void FadeToVisible(Action? posthook = null)
    {
        StartCoroutine(C_FadeToVisible(posthook));
    }

    public void FadeInAndOut(Action? posthook = null)
    {
        StartCoroutine(C_FadeInAndOut(posthook));
    }
    public void FadeInAndOutCallback(Action? prehook = null, Action? midhook = null, Action? posthook = null, float? blackScreenTime = 0f)
    {
        StartCoroutine(C_FadeInAndOut(prehook, midhook, posthook,  blackScreenTime));
    }

    IEnumerator C_FadeInAndOut(Action? prehook = null, Action? midhook = null, Action? posthook = null, float? blackScreenTime = 0f)
    {
        if (isFading) yield break;
        prehook?.Invoke();

        yield return StartCoroutine(C_FadeToBlack(midhook));

        float fadeDuration = (1f / fadeStep) * incrementDelay;
        yield return new WaitForSeconds(fadeDuration * 0.5f);

        if(blackScreenTime > 0f) yield return new WaitForSeconds((float)blackScreenTime);

        yield return StartCoroutine(C_FadeToVisible(posthook));
    }



    IEnumerator C_FadeToBlack(Action? posthook = null)
    {
        isFading = true;
        Color fade = panel.color;
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
        posthook?.Invoke();
    }

    IEnumerator C_FadeToVisible(Action? posthook = null)
    {
        isFading = true;
        Color fade = panel.color;
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
        posthook?.Invoke();
    }

    public void SetVisble()
    {
        Color color = panel.color;
        color.a = 0f;
        panel.color = color;
    }

    public void SetOpaque()
    {
        Color color = panel.color;
        color.a = 1f;
        panel.color = color;
    }
}