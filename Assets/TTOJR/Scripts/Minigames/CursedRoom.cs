using DependencyInjection;
using UnityEngine;
using Extensions;
using static Extensions.AudioEX;

public class CursedRoom : RuntimeInjectableMonoBehaviour
{
    #region Privates
    [Inject] Referencer referencer;
    ParticleSystem frostEffect;
    GameObject frostScreen;
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
        OutOfRange();
        cursed = true;
    }

    public void Uncurse()
    {
        cursed = false;
        OutOfRange();
    }

    public void OutOfRange()
    {
        frostEffect.gameObject.SetActive(false);
        frostScreen.SetActive(false);
        breathSource.Stop();
    }

    public void InRange()
    {
        if (!cursed) return;
        frostScreen.SetActive(true);
        freezeSource.Play(initialFreezeSFX);
        breathSource.Play(breathingSFX, false);
        frostEffect.gameObject.SetActive(true);
        frostEffect.Play();
    }
}
