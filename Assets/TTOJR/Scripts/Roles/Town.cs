using System.Collections.Generic;
using DependencyInjection;
using Extensions;
using NodeCanvas.DialogueTrees;
using Sirenix.OdinInspector;
using UnityEngine;

public class Town : RuntimeInjectableMonoBehaviour
{
    [SerializeField][Inject] public EntityControls player;

    #region Privates
    [Inject] TimeCycle timeCy;
    [SerializeField] bool _corrupted;
    bool hasSpawnedInCorruptedAlready = false;
    #endregion

    public List<Quest> activityQuests;
    public int currentCorruptionLevel = 0;
    public bool corrupted { get => _corrupted; set => _corrupted = value; }

    GameObject _playerObj;
    public GameObject playerObj { get => player.gameObject; set => _playerObj = value; }

    float _distToPlayer;
    public float distToPlayer
    {
        get
        {
            if (!playerObj) return default;
            return _distToPlayer = Vector3.Distance(transform.position, playerObj.transform.position);  
        }

        set => _distToPlayer = value;
    }

    private void OnEnable()
    {
        playerObj = player.gameObject;

        if (corrupted && timeCy.IsDay()) gameObject.SetActive(false); //Doesnt spawn corrupted Town during day
        if (corrupted && timeCy.IsNight()) //Spawned corrupted can only spawn at night, and only once, then will revert back one corrupted levevl
        {
            if (!hasSpawnedInCorruptedAlready)
                hasSpawnedInCorruptedAlready = true;
            else
            {
                DisableCorruptedFunctionality();
                corrupted = false;
                currentCorruptionLevel--;
                return;
            }
        }

        if (!corrupted && currentCorruptionLevel >= 3) Corrupt();
        if (corrupted) EnableCorruptedFunctionality();

    }

    [Button]
    public void IncreaseCorruption()
    {
        currentCorruptionLevel++;
    }

    public void Corrupt() => corrupted = true;

    public void EnableCorruptedFunctionality()
    {
        var dialauge = this.TryGet<Dialuage>();
        var npcMovement = this.TryGet<NPC_Movement>();
        var dialaugeActor = this.TryGet<DialogueActor>();
        var dialaugeTree = this.TryGet<DialogueTreeController>();

        dialaugeTree.enabled = false;
        dialaugeActor.enabled = false;
        dialauge.enabled = false;
        npcMovement.enabled = false;
    }

    public void DisableCorruptedFunctionality()
    {
        var dialauge = this.TryGet<Dialuage>();
        var npcMovement = this.TryGet<NPC_Movement>();
        var dialaugeActor = this.TryGet<DialogueActor>();
        var dialaugeTree = this.TryGet<DialogueTreeController>();

        dialaugeTree.enabled = true;
        dialaugeActor.enabled = true;
        dialauge.enabled = true;
        npcMovement.enabled = true;
    }

    public void BecomeResident()
    {

    }


    #region Methods
        
    #endregion

}
