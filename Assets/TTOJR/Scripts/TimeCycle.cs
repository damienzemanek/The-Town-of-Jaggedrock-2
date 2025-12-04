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
    public int difficultyLevel;
    public float[] stepBetweenCorruptEvents;
    public bool timeFrozen = false;

    private void Start()
    {
        currentTime = 50f;
    }

    private void FixedUpdate()
    {
        if (timeFrozen) return;
        currentTime += Time.deltaTime;
        if (CorruptAvaliable()) Corrupt();
    }

    bool CorruptAvaliable()
    {
        if (currentTime > stepBetweenCorruptEvents[difficultyLevel]) return true;
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
        currentTime = stepBetweenCorruptEvents[difficultyLevel];
    }

    public void IncreaseDifficulty()
    {
        if (difficultyLevel < stepBetweenCorruptEvents.Length - 1)
            difficultyLevel++;
    }

    public void DecreaseDifficulty()
    {
        if (difficultyLevel > 1)
            difficultyLevel -= 2;
    }

}
