using UnityEngine;
using DependencyInjection;
using UnityEngine.AI;
using NodeCanvas.DialogueTrees;
using Sirenix.OdinInspector;
using Extensions;

[DefaultExecutionOrder(1)]
[RequireComponent(typeof(CallbackDetector))]
public class Dialuage : RuntimeInjectableMonoBehaviour, ICallbackUser
{
    #region Privates
    [Inject] [ReadOnly, ShowInInspector] EntityControls playerControls;
    [Inject] [ReadOnly, ShowInInspector] Interactor interactor;
    #endregion


#pragma warning disable IDE0052 
    [TabGroup("Readonly"), ReadOnly, SerializeField] bool inConvo = false;

#pragma warning restore IDE0052 

    [TabGroup("Parameters")][SerializeField] SO_Person person;
    [field:TabGroup("Parameters")][field:SerializeField] public SO_Favor favor { get; private set; }

    [TabGroup("Visual")][SerializeField] GameObject isTalkingEffectPrefab;
    [TabGroup("Visual")][SerializeField, ReadOnly] GameObject isTalkingEffect;
    [TabGroup("Visual")][SerializeField] Transform effectLoc;


    public SO_Person so_person { get => person; set => person = value; }
    public string mname { get => (person != null) ? person.personName : string.Empty; }
    public SO_Favor.FavorStatus GetFavorStatus => favor.status;

    #region Privs
    CallbackDetector detector;
    DialogueTreeController dialaugeController;
    DialogueActor actor;
    bool checkingEndGame = false;
    #endregion

    protected override void OnInstantiate()
    {
        base.OnInstantiate();
        detector = this.Get<CallbackDetector>();
        dialaugeController = this.Get<DialogueTreeController>();
        actor = this.Get<DialogueActor>();
        AssignValuesForCallbackDetector("Talk (E)");
        AssignDialaugeActorName();
        checkingEndGame = false;

        if (isTalkingEffect == null) isTalkingEffect = Instantiate(isTalkingEffectPrefab, effectLoc).SetActiveThen(false);
    }


    public void AssignValuesForCallbackDetector(string interactText)
    {
        detector.Stay.AddListener(() => interactor.SetInteractText(interactText));
        detector.Stay.AddListener(() => interactor.ToggleCanInteract(true));
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
        if (inConvo) return;

        if(CorruptionManager.instance.lost)
        {
            CorruptionManager.instance.TransitionToLoseScreen();
            return;
        }

        StartDialauge();
        TogglePlayerMovement(false);
        PlayerLooksAtMe();
        SetTalkEffectsActive(true);
    }



    void TogglePlayerMovement(bool val) => playerControls.canMove = val;
    void PlayerLooksAtMe()
    {
        Look look = playerControls.Get<Look>();
        Inventory inv = playerControls.Get<Inventory>();

        playerControls.headDirection.transform.LookAt(transform.position.With(y: 4f));
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
        isTalkingEffect.SetActive(value: false);

        if (checkingEndGame) return;

        Look look = playerControls.Get<Look>();
        Inventory inv = playerControls.Get<Inventory>();

        inConvo = false;
        TogglePlayerMovement(true);
        look.ToggleCursorUsability(false);
        look.ToggleUpdateMouseLooking(true);
        inv.ToggleInventoryVisability(true);

    }
    void AssignDialaugeActorName() => actor.AssignName(input_name: mname);

    public void SetTalkEffectsActive(bool val)
    {
        isTalkingEffect.SetActive(val);
    }

    public void GetShot()
    {
        checkingEndGame = true;
        EvilInformationManager.Instance.AttemptShoot(this);

    }
}

