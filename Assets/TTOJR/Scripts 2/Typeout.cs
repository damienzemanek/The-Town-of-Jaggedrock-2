using System.Collections;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using Extensions;
using System;
using static Extensions.FadeEX;

public class Typeout : MonoBehaviour
{
    public bool toMenu;
    public LoadScene scene;
    public FadeSettings fade;
    [ShowIf("@!toMenu")] public FadeSettings selfFade;

    public TextMeshProUGUI tmp;
    public string text;
    public float delay;
    public int speedupPoint;
    public float spedUpDelay;

    private void Awake()
    {
        scene = LoadScene.Instance;
    }

    [Button]
    private void OnEnable()
    {
        if(toMenu)
            StartCoroutine(Type(() => StartCoroutine(C_FadeToTransparent(fade, ToMenu))));
        else
            StartCoroutine(Type(() => StartCoroutine(C_FadeToTransparent(fade, FadeOutSelf))));

    }

    void ToMenu()
    {
        scene.LoadImmediate(0);
    }

    void FadeOutSelf()
    {
        StartCoroutine(C_FadeToTransparent(selfFade));
    }

    public IEnumerator Type(Action posthook = null)
    {
        string currentText = "";
        int indx = 0;

        while(indx < text.Length)
        {
            currentText += text[indx];
            indx++;
            tmp.text = currentText;

            if (indx == speedupPoint) delay = spedUpDelay;

            yield return new WaitForSeconds(delay);
        }

        posthook?.Invoke();
    }


}
