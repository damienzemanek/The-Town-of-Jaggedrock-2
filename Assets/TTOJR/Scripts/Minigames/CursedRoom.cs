using UnityEngine;

public class CursedRoom : MonoBehaviour
{
    public bool cursed;
    public ParticleSystem frostEffect;

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
