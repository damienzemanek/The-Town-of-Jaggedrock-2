using System;
using System.Collections;
using DependencyInjection;
using Extensions;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Corrupted : MonoBehaviour
{

    #region Privates
    [SerializeField] GameObject attackObj;
    [SerializeField] float attackLength;
    [SerializeField] AudioPlay audioPlay;
    [SerializeField] AudioClip[] attackAudioClips;
    [SerializeField] AudioClip[] growlAmbienceClips;

    bool attacking = false;

    #endregion

    private void Awake()
    {
        audioPlay = this.TryGetOrAdd<AudioPlay>();
        if (attackAudioClips == null || attackAudioClips.Length == 0) this.Error("Attack audio clips need setting");
    }

    private void OnEnable()
    {
        attackObj.SetActive(false);
        StartCoroutine(C_CorruptedAmbienceNoises());
    }

    public IEnumerator C_CorruptedAmbienceNoises()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(EnumerateEX.Rand(3, 5));
            audioPlay.PlayRand(growlAmbienceClips);
        }
    }

    public void Attack()
    {
        StartCoroutine(C_Attack());
    }

    IEnumerator C_Attack()
    {
        if (attacking) yield break;

        attacking = true;
        attackObj.SetActive(true);
        audioPlay.PlayRand(attackAudioClips);
        yield return new WaitForSeconds(attackLength);
        attackObj.SetActive(false);
        attacking = false;
    }



    #region Methods



    #endregion

}


