using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using Sirenix.OdinInspector;
using UnityEngine;


public static class Effectability
{
    [Serializable]
    public struct EffectUser
    {
        [SerializeField] ParticleSystem effect;
        [SerializeField] float effectLength;

        public void UseEffect()
        {
            ParticleSystem e = effect;
            float length = effectLength;

            ParticleSystem.MainModule main = e.main;
            main.loop = false;
            main.duration = effectLength;

            e.gameObject.SetActive(true);
            e.Play();
            if (effect.gameObject.Has(out AudioSource source)) source.Play();
        }
    }

    [Serializable]
    public struct EffectObjectRagdoll
    {
        [SerializeField] GameObject[] prePlacedObjects;
        [SerializeField] float lifetime;

        public void UseEffect()
        {
            prePlacedObjects.SetAllActive(true);
            foreach (GameObject obj in prePlacedObjects)
                GameObject.Destroy(obj, lifetime);
        }
    }

}

