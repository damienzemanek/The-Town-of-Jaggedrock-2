using UnityEngine;
using DependencyInjection;
using UnityEngine.AI;
using NodeCanvas.DialogueTrees;
using Sirenix.OdinInspector;
using Extensions;

[RequireComponent(typeof(CallbackDetector))]
public class Dialuage : RuntimeInjectableMonoBehaviour, ICallbackUser
{
    #region Privates
    [Inject] EntityControls playerControls;
    [Inject] Interactor interactor;
    #endregion


#pragma warning disable IDE0052 
    [TabGroup("Readonly"), ReadOnly, SerializeField] bool inConvo = false;
    [TabGroup("Readonly"), SerializeField] public bool initialChatComplete = false;
    [TabGroup("Readonly"), ReadOnly, SerializeField] public bool completedTalkingTo = false;

#pragma warning restore IDE0052 

    [TabGroup("Parameters")][SerializeField] SO_Person person;
    [field:TabGroup("Parameters")][field:SerializeField] public SO_Favor favor { get; private set; }
    [TabGroup("Parameters")] public bool willCompleteTalkingToAfterInitialDialauge;

    [TabGroup("Visual")][SerializeField] GameObject needsTalkingEffectPrefab;
    [TabGroup("Visual")][SerializeField] GameObject isTalkingEffectPrefab;
    [TabGroup("Visual")][SerializeField, ReadOnly] GameObject needsTalkingEffect;
    [TabGroup("Visual")][SerializeField, ReadOnly] GameObject isTalkingEffect;
    [TabGroup("Visual")][SerializeField] Transform effectLoc;


    public SO_Person so_person { get => person; set => person = value; }
    public string personName { get => (person != null) ? person.personName : string.Empty; }
    public SO_Favor.FavorStatus GetFavorStatus => favor.status;

    #region Privs
    CallbackDetector detector;
    NPC_Movement movement;
    NavMeshAgent agent;
    DialogueTreeController dialaugeController;
    DialaugeChooser dialaugeChooser;
    DialogueActor actor;
    #endregion

    protected override void OnInstantiate()
    {
        base.OnInstantiate();
        detector = this.TryGet<CallbackDetector>();
        movement = this.TryGet<NPC_Movement>();
        agent = this.TryGet<NavMeshAgent>();
        dialaugeController = this.TryGet<DialogueTreeController>();
        dialaugeChooser = this.TryGet<DialaugeChooser>();
        actor = this.TryGet<DialogueActor>();
        AssignValuesForCallbackDetector("Talk (E)");
        AssignDialaugeActorName();

        if (needsTalkingEffect == null) needsTalkingEffect = Instantiate(needsTalkingEffectPrefab, effectLoc).SetActiveThen(false);
        if (isTalkingEffect == null) isTalkingEffect = Instantiate(isTalkingEffectPrefab, effectLoc).SetActiveThen(false);
    }

    private void OnEnable()
    {
        completedTalkingTo = false;
        needsTalkingEffect.SetActive(true);
    }


    public void AssignValuesForCallbackDetector(string interactText)
    {
        detector.Enter.AddListener(() => interactor.SetInteractText(interactText));
        detector.Enter.AddListener(() => interactor.ToggleCanInteract(true));
        detector.Exit.AddListener(call: () => interactor.ToggleCanInteract(false));
        detector.useCallback.AddListener(() => interactor.ToggleCanInteract(false));
        detector.useCallback.AddListener(DialaugeUsage);
    }

    public void GainMinorFavor() => favor.GainMinorFavor();
    public void GainMajorFavor() => favor.GainMajorFavor();
    public void LoseMinorFavor() => favor.LoseMinorFavor();
    public void LoseMajorFavor() => favor.LoseMajorFavor();

    void DialaugeUsage()
    {
        if (completedTalkingTo) return;
        StartDialauge();
        DisableMyMovement();
        TogglePlayerMovement(false);
        LookAtWhoImTalkingTo();
        TalkeeLooksAtMe();
        FreezeTime(true);
        EnableTalkingVisuals();
    }

    void EnableTalkingVisuals()
    {
        needsTalkingEffect.SetActive(false);
        isTalkingEffect.SetActive(true);
    }
    void LookAtWhoImTalkingTo()
    {
        transform.LookAtPosThenMyTransform(playerControls.transform.position.With(y: 0))
            .WithEuler(x: 0, z: 0);
    }
    void DisableMyMovement()
    {
        movement.enabled = false;
        agent.enabled = false;
    }

    void TogglePlayerMovement(bool val) => playerControls.canMove = val;
    void TalkeeLooksAtMe()
    {
        Look look = playerControls.TryGet<Look>();
        Inventory inv = playerControls.TryGet<Inventory>();

        playerControls.headDirection.transform.LookAt(transform.position.With(y: 3));
        look.ToggleCursorUsability(true);
        look.ToggleUpdateMouseLooking(false);
        inv.ToggleInventoryVisability(false);
    }
    void StartDialauge()
    {
        if (dialaugeController == null) this.Error("dialaugeOwner is null");
        inConvo = true;
        dialaugeController.StartDialogue(StopDialauge);
    }
    void StopDialauge(bool success)
    {
        Look look = playerControls.TryGet<Look>();
        Inventory inv = playerControls.TryGet<Inventory>();

        TogglePlayerMovement(true);
        look.ToggleCursorUsability(false);
        look.ToggleUpdateMouseLooking(true);
        inv.ToggleInventoryVisability(true);

        movement.enabled = true;
        agent.enabled = true;
        inConvo = false;

        FreezeTime(false);
        PotentiallyCompleteDialauge();
        isTalkingEffect.SetActive(value: false);
        if (completedTalkingTo) needsTalkingEffect.SetActive(false);
    }
    void AssignDialaugeActorName() => actor.AssignName(input_name: personName);
    void FreezeTime(bool val) => TimeCycle.instance.timeFrozen = val;
    void PotentiallyCompleteDialauge()
    {
        if (willCompleteTalkingToAfterInitialDialauge) CompleteDialague();
    }

    public void CompleteDialague() => completedTalkingTo = true;

    public void SetTalkEffectsActive(bool val)
    {
        needsTalkingEffect.SetActive(val);
        isTalkingEffect.SetActive(val);
    }
}

