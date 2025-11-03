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
    bool attacking = false;

    #endregion

    private void Awake()
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
        yield return new WaitForSeconds(attackLength);
        attackObj.SetActive(false);
        attacking = false;
    }



    #region Methods



    #endregion

}


