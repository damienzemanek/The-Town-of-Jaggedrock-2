using DependencyInjection;
using UnityEngine;
using Extensions;
using static Extensions.AudioEX;

public class CursedRoom : RuntimeInjectableMonoBehaviour
{
    #region Privates
    [Inject] Referencer referencer;
    ParticleSystem frostEffect;
    FadeIn frostScreen;
    #endregion

    [SerializeField] bool _cursed;
    [SerializeField] AudioSource freezeSource;
    [SerializeField] AudioSource breathSource;
    [SerializeField] AudioClip initialFreezeSFX;
    [SerializeField] AudioClip breathingSFX;
    public bool cursed { get => _cursed; set => _cursed = value; }

    private void Start()
    {
        frostEffect = referencer.frostEffect.Get<ParticleSystem>();
        frostScreen = referencer.frostScreen;
        frostEffect.gameObject.SetActive(false);
        breathSource.Stop();
        cursed = true;
    }

    public void Uncurse()
    {
        frostEffect.gameObject.SetActive(false);
        cursed = false;
        OutOfRange();
    }

    public void OutOfRange()
    {
        frostEffect.gameObject.SetActive(false);
        frostScreen.DoFadeOut();
        breathSource.Stop();
    }

    public void InRange()
    {
        if (!cursed) return;
        frostScreen.DoFadeIn();
        freezeSource.Play(initialFreezeSFX);
        breathSource.Play(breathingSFX, false);
        frostEffect.gameObject.SetActive(true);
        frostEffect.Play();
    }

    public void SelfDestroy()
    {
        OutOfRange();
        Destroy(gameObject);
    }
}
