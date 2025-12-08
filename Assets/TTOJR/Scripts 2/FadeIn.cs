using UnityEngine;
using Extensions;
using static Extensions.FadeEX;

public class FadeIn : MonoBehaviour
{
    public FadeSettings fade;
    public bool startTransparent;

    private void Start()
    {
        if (startTransparent) fade.SetAlpha(0);
        else fade.SetAlpha(fade.finalPercentage);
    }

    public void DoFadeIn()
    {
        fade.GetGO()?.SetActive(true);
        fade.SetAlpha(0);
        StartCoroutine(C_FadeToOpaque(fade));
    }

    public void DoFadeOut()
    {
        StartCoroutine(C_FadeToTransparent(fade, () =>
        {
            fade.SetAlpha(0);
            fade.GetGO()?.SetActive(false);
        }));

    }
}
