using DependencyInjection;
using UnityEngine;
using Extensions;

public class CursedRoom : RuntimeInjectableMonoBehaviour
{
    #region Privates
    [Inject] Referencer referencer;
    ParticleSystem frostEffect;
    #endregion

    [SerializeField] bool _cursed;
    public bool cursed { get => _cursed; set => _cursed = value; }

    private void Start()
    {
        frostEffect = referencer.frostEffect.Get<ParticleSystem>();
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
    }

    public void InRange()
    {
        if (!cursed) return;
        frostEffect.gameObject.SetActive(true);
        frostEffect.Play();
    }
}
