using System.Collections.Generic;
using DependencyInjection;
using Extensions;
using NodeCanvas.DialogueTrees;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(300)]
public class Town : RuntimeInjectableMonoBehaviour
{
    [SerializeField][Inject] public EntityControls player;

    #region Privates
    [Inject] TimeCycle timeCy;
    [SerializeField] bool _corrupted;
    [SerializeField] bool hasSpawnedInCorruptedAlready = false;
    #endregion

    public List<Quest> activityQuests;
    public float regularSpeed = 3.5f;
    public float baseCorruptedSpeed = 2f;
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

    protected override void OnInstantiate()
    {
        base.OnInstantiate();
        playerObj = player.gameObject;
    }

    private void Start()
    {
        hasSpawnedInCorruptedAlready = false;
    }


    private void OnEnable()
    {
        if (!corrupted && currentCorruptionLevel >= 3) Corrupt();
        if (corrupted) EnableCorruptedFunctionality();


        print("1");

        if (corrupted) print("corrupted");
        if (timeCy.IsDay()) print("time day");

        if (corrupted && timeCy.IsDay()) //Doesnt spawn corrupted Town during day
        {
            print("2");

            if (!hasSpawnedInCorruptedAlready)
            {
                gameObject.SetActive(false);
                return;
            }
            else
            {
                RevertCorruption();
                print("3");

                return;
            }
        }

        if (corrupted && timeCy.IsNight()) //Spawned corrupted can only spawn at night, and only once, then will revert back one corrupted levevl
        {
            print("4");

            if (!hasSpawnedInCorruptedAlready)
            {
                hasSpawnedInCorruptedAlready = true;
                return;
            }
            else
            {
                RevertCorruption();
                print(message: "5");

                return;

            }
        }
        print("6");

    }

    void RevertCorruption()
    {
        DisableCorruptedFunctionality();
        corrupted = false;
        currentCorruptionLevel--;
    }

    [Button]
    public void IncreaseCorruption()
    {
        currentCorruptionLevel++;
    }

    public void Corrupt() => corrupted = true;

    public void EnableCorruptedFunctionality()
    {
        var dialauge = this.Get<Dialuage>();
        var npcMovement = this.Get<NPC_Movement>();
        var dialaugeActor = this.Get<DialogueActor>();
        var dialaugeTree = this.Get<DialogueTreeController>();
        var cbd = this.Get<CallbackDetector>();

        dialaugeTree.enabled = false;
        dialaugeActor.enabled = false;
        dialauge.enabled = false;
        npcMovement.enabled = false;
        cbd.enabled = false;
        this.Get<Dialuage>().SetTalkEffectsActive(false);
        this.Get<NavMeshAgent>().speed = baseCorruptedSpeed;
    }

    public void DisableCorruptedFunctionality()
    {
        var dialauge = this.Get<Dialuage>();
        var npcMovement = this.Get<NPC_Movement>();
        var dialaugeActor = this.Get<DialogueActor>();
        var dialaugeTree = this.Get<DialogueTreeController>();
        var cbd = this.Get<CallbackDetector>();


        dialaugeTree.enabled = true;
        dialaugeActor.enabled = true;
        dialauge.enabled = true;
        npcMovement.enabled = true;
        cbd.enabled = true;
        this.Get<NavMeshAgent>().speed = regularSpeed;

    }

    public void BecomeResident()
    {

    }


    #region Methods
        
    #endregion

}
