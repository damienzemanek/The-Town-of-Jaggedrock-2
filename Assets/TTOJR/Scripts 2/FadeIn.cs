using UnityEngine;
using Extensions;
using static Extensions.FadeEX;

public class FadeIn : MonoBehaviour
{
    public FadeSettings fade;

    private void Start()
    {
        fade.GetGO()?.SetActive(true);
        fade.SetAlpha(0);
        StartCoroutine(C_FadeToOpaque(fade));
    }

    private void OnDisable()
    {
        fade.GetGO()?.SetActive(false);
        fade.SetAlpha(0);
    }
}
