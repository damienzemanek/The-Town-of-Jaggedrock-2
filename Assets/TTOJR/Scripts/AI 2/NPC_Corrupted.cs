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
    [SerializeField] AudioPlay attackAudioPlayer;
    [SerializeField] AudioClip[] attackAudioClips;
    bool attacking = false;

    #endregion

    private void Awake()
    {
        attackAudioPlayer = this.TryGetOrAdd<AudioPlay>();
        if (attackAudioClips == null || attackAudioClips.Length == 0) this.Error("Attack audio clips need setting");
    }

    private void OnEnable()
    {
        attackObj.SetActive(false);
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
        attackAudioPlayer.PlayRand(attackAudioClips);
        yield return new WaitForSeconds(attackLength);
        attackObj.SetActive(false);
        attacking = false;
    }



    #region Methods



    #endregion

}


