using UnityEngine;
using Extensions;
using static Extensions.FadeEX;

public class FadeIn : MonoBehaviour
{
    public FadeSettings fade;


    private void OnEnable()
    {
        fade.GetGO()?.SetActive(true);
        fade.SetAlpha(0);
        StartCoroutine(C_FadeToOpaque(fade));
    }

    private void OnDisable()
    {
        StartCoroutine(C_FadeToTransparent(fade, () =>
        {
            fade.SetAlpha(0);
            fade.GetGO()?.SetActive(false);
        }));

    }
}
