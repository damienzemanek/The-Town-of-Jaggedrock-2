using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Extensions;
using TMPro;
using Sirenix.Utilities;
using DesignPatterns.CreationalPatterns;

public class TimeCycle : Singleton<TimeCycle>
{
    #region Member Classes
    #endregion

    [Inject] EntityControls controls;

    public float currentTime;
    public float stepBetweenCorruptEvents = 80f;
    public bool timeFrozen = false;

    private void FixedUpdate()
    {
        if (timeFrozen) return;
        currentTime += Time.deltaTime;
        if (CorruptAvaliable()) Corrupt();
    }

    bool CorruptAvaliable()
    {
        if (currentTime > stepBetweenCorruptEvents) return true;
        else return false;
    }

    void Corrupt()
    {
        currentTime = 0;
        CorruptionManager.instance.CorruptNext();
    }

    [Button]
    void AutoCorrupt()
    {
        currentTime = stepBetweenCorruptEvents;
    }



}
