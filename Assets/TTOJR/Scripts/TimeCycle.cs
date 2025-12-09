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
using DesignPatterns.CreationalPatterns;
using static Extensions.AudioEX;

public class TimeCycle : Singleton<TimeCycle>
{
    #region Member Classes
    #endregion

    [Inject] EntityControls controls;

    public float currentTime;
    public int difficultyLevel;
    public float[] stepBetweenCorruptEvents;
    public bool timeFrozen = false;
    public TypeoutMulti intro;

    [TabGroup("Audio")] public AudioSource source;
    [TabGroup("Audio")] public AudioClip[] startCorruption;
    [TabGroup("Audio")] public AudioClip[] stopCorruption;
    [TabGroup("Audio")] public AmbiencePlayer ambience;

    protected override void Awake()
    {
        base.Awake();
        if (ambience == null) ambience = FindAnyObjectByType<AmbiencePlayer>();
    }

    private void Start()
    {
        currentTime = 35f;
        ambience.PlayGeneralAmbience();
    }

    private void Update()
    {
        if (!intro.complete)
        {
            if (Input.GetKeyDown(KeyCode.Return))
                intro.Skip();

        }
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

    public void HaltCorrupt()
    {
        source.Play(stopCorruption.Rand());
    }

    void Corrupt()
    {
        currentTime = 0;
        CorruptionManager.instance.CorruptNext();
        source.Play(startCorruption.Rand());
        this.DelayedCall(() => ambience.PlayCorruptingAmbience(), 1f);
    }

    [Button]
    void AutoCorrupt()
    {
        currentTime = stepBetweenCorruptEvents[difficultyLevel];
    }

    public void IncreaseDifficulty()
    {
        difficultyLevel++;

        if (difficultyLevel >= stepBetweenCorruptEvents.Length)
            difficultyLevel = 0;
    }

    public void DecreaseDifficulty()
    {
        if (difficultyLevel > 1)
            difficultyLevel -= 2;
    }

}
