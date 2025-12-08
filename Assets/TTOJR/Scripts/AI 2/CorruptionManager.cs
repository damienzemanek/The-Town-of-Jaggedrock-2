using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class CorruptionManager : MonoBehaviour
{
    public static CorruptionManager instance;
    public bool lost = false;

    [Serializable]
    public struct CorruptEvent
    {
        public bool isRandom;
        bool isntRandom { get => !isRandom; }
        [ShowIf("isntRandom")] public UnityEvent corruptHook;
    }

    #region Privates
    [SerializeField] FadeScreen afflictBg;
    [SerializeField] FadeScreen afflictFadeInto;
    [SerializeField] GameObject afflictVisual;
    [SerializeField] int currentCorruption;
    #endregion

    [TabGroup("Residents")] public List<Town> residents;
    [TabGroup("Locations")] public List<CorruptonLocation> corruptionLocations;
    [TabGroup("Types of Events")][SerializeReference] public List<CorruptEventType> corruptEventTypes;
    [TabGroup("Actual Events")] public List<CorruptEvent> corruptionEvents;
    
    [SerializeField] SceneChange scene;
    public Material[] corrMats = new Material[3];
    public Material fullyCorrupt;
    public Material finishedGameMat;
    public Material chandelierMat;
    public List<GameObject> lights;
    public FadeIn vignet;

    public UnityEvent onCorrupted;

    private void Awake()
    {
        lost = false;
        instance = this;
        if(onCorrupted == null) onCorrupted = new UnityEvent();
        if (corruptionLocations == null || corruptionLocations.Count == 0)
            corruptionLocations = gameObject.GetComponentsInChildren<CorruptonLocation>().ToList();
        if (corruptEventTypes == null || corruptEventTypes.Count == 0)
            this.Error("Corrupt Events not set, please assign");
    }

    private void OnEnable()
    {
        onCorrupted.AddListener(AfflictResidentDisplay);
    }

    private void OnDisable()
    {
        onCorrupted.RemoveAllListeners();
    }

    public void CorruptNext()
    {
        if (corruptionEvents[currentCorruption].isRandom) CorruptRandom();
        else throw new NotImplementedException("Non random corrupt events not implemented yet");
    }

    public void CorruptRandom()
    {
        CorruptonLocation loc = corruptionLocations.Rand();

        if(loc.corrupting) { CorruptRandom(); return; }

        loc.StartCorruption();
        vignet.DoFadeIn();
    }

    public void CorruptCompelte()
    {
        this.Log("Corrupt Complete");
        onCorrupted?.Invoke();
        AfflictResidentDisplay();
        TimeCycle.Instance.DecreaseDifficulty();
        vignet.DoFadeOut();
    }

    public void CorruptHalted()
    {
        this.Log("Corrupt Halted");
        TimeCycle.Instance.IncreaseDifficulty();
        TimeCycle.Instance.HaltCorrupt();
        vignet.DoFadeOut();
    }

    public void AfflictResidentDisplay()
    {
        afflictBg.gameObject.SetActive(true);
        afflictFadeInto.gameObject.SetActive(true);
        afflictVisual.SetActive(true);
        afflictFadeInto.FadeToVisible(() => this.DelayedCall(AfflictStop, 2));
    }

    void AfflictStop()
    {
        afflictFadeInto.FadeToBlack(() =>
        {
            afflictFadeInto.gameObject.SetActive(false);
            afflictVisual.SetActive(false);
            afflictBg.FadeToVisible(() =>
            {
                afflictBg.gameObject.SetActive(false);
                afflictBg.SetOpaque();
            });

        });
    }

    public void LoseGame()
    {
        foreach(Town resident in residents)
            if(!resident.corrupted)
                resident.obj.Get<Renderer>().material = finishedGameMat;

        foreach (GameObject l in lights)
        {
            l.gameObject.SetActive(false);
        }

        chandelierMat?.DisableKeyword("_EMISSION");

        TimeCycle.Instance.timeFrozen = true;
        lost = true;
    }

    public void TransitionToLoseScreen()
    {
        scene.ChangeScene("Lose");
    }


    #region Methods

    #endregion

}
