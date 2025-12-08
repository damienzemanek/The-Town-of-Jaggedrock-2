using Extensions;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ProBuilder;
using static Extensions.FadeEX;

public class TypeoutMulti : MonoBehaviour
{
    public UnityEvent AfterAllTypeoutsComplete;
    public FadeSettings panelFade;
    [SerializeField] float delayBetweenTypes;

    [Serializable]
    public struct TypeoutText
    {
        public FadeSettings textfade;
        public TextMeshProUGUI tmps;
        public string text;
        public float delay;
    }

    public TypeoutText[] type;


    public void StartTypeout()
    {
        StartCoroutine(Type(() =>
        {
            foreach(var t in type)
            {
                if (t.Equals(type[type.Length - 1]))
                    StartCoroutine(C_FadeToTransparent(t.textfade, FadeOutSelf));
                else
                    StartCoroutine(C_FadeToTransparent(t.textfade));

            }
        }));
    }

    void FadeOutSelf()
    {
        print("Fading out self");
        StartCoroutine(C_FadeToTransparent(panelFade));
        AfterAllTypeoutsComplete?.Invoke();
    }

    public IEnumerator Type(Action posthook = null)
    {
        for (int i = 0; i < type.Length; i++)
        {
            var t = type[i];
            if (t.tmps == null || string.IsNullOrEmpty(t.text))
                continue;

            string currentText = "";
            int indx = 0;
            t.tmps.text = "";

            while (indx < t.text.Length)
            {
                currentText += t.text[indx];
                indx++;
                t.tmps.text = currentText;

                yield return new WaitForSeconds(t.delay);
            }

            yield return new WaitForSeconds(delayBetweenTypes);

        }

        posthook?.Invoke();
    }
}


