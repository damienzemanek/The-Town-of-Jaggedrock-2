using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Extensions;
using System;
using System.Reflection.Metadata.Ecma335;

public class CallbackDetector : Detector, IAssigner
{
    #region Privates
    public enum CallbackFunctionality
    {
        singleCallback,
        toggleCallback
    }
    bool singleCbCheck() => (functionality == CallbackFunctionality.singleCallback);
    bool toggleCbCheck() => (functionality == CallbackFunctionality.toggleCallback);

    #endregion

    [Title("Callback Detector")]
    public CallbackFunctionality functionality;
    public bool holdingUseDetector;

    [ShowIf(nameof(singleCbCheck))]
    public UnityEvent useCallback;

    [ShowIf(nameof(singleCbCheck))]
     public UnityEvent holdCancledCallback;

    [ShowIf(nameof(toggleCbCheck))] 
    [field: SerializeReference]
    public UnityEvent[] toggleUseCallback;

    [ShowIf(nameof(toggleCbCheck))]
    [field: SerializeReference]
    public UnityEvent[] toggleUseCancledCallback;

    [ShowIf(nameof(toggleCbCheck))]
    [SerializeField]
    int currCallback = 0;

    public class Builder
    {
        readonly GameObject objOn;
        readonly CallbackDetector cbd;

        public Builder(GameObject on)
        {
            objOn = on ?? throw new System.ArgumentNullException(nameof(on));
            cbd = objOn.TryGetOrAdd<CallbackDetector>();
            cbd.useCallback = new UnityEvent();
        }

        public Builder WithRaycast()
        {
            cbd.rayCastDetector = true;
            return this;
        }

        public Builder WithCollision()
        {
            cbd.collisionDetector = true;
            return this;
        }

        public Builder WithEventHooks(bool? enter = null, bool? stay = null, bool? exit = null)
        {
            cbd.Init(enter, stay, exit);
            cbd.useCallback = new UnityEvent();
            return this;
        }

        public Builder WithHoldingUse(int toggleAmount = 2)
        {
            cbd.holdingUseDetector = true;
            cbd.holdCancledCallback = new UnityEvent();

            if (cbd.toggleCbCheck())
            {
                cbd.toggleUseCancledCallback = new UnityEvent[toggleAmount];
                for (int i = 0; i < toggleAmount; i++)
                    cbd.toggleUseCancledCallback[i] = new UnityEvent();
            }

            return this;
        }

        public Builder WithToggleCallback(int toggleAmount = 2)
        {
            cbd.functionality = CallbackFunctionality.toggleCallback;
            cbd.toggleUseCallback = new UnityEvent[toggleAmount];
            for(int i = 0; i < toggleAmount; i ++)
                cbd.toggleUseCallback[i] = new UnityEvent();
            return this;
        }

        public Builder WithInteractAssignments(Interactor interactor, string interactMessage)
        {
            if(cbd.Stay == null) cbd.Stay = new UnityEvent();
            if(cbd.Exit == null) cbd.Exit = new UnityEvent();
            if(cbd.useCallback == null) cbd.useCallback = new UnityEvent();

            cbd.Stay.AddListener(() => interactor.SetInteractText(interactMessage));
            cbd.Stay.AddListener(() => interactor.ToggleCanInteract(true));
            cbd.Exit.AddListener(() => interactor.ToggleCanInteract(false));
            cbd.useCallback.AddListener(() => interactor.ToggleCanInteract(false));

            return this;
        }

        public CallbackDetector Build()
        {
            return cbd;
        }
    }

    private void Awake()
    {
        if(toggleCbCheck())
            for(int i = 0; i < toggleUseCallback.Length; i++)
                toggleUseCallback[i].AddListener(ToggleCallback);

        gameObject.layer = LayerMask.NameToLayer("Interactable");
        useCallback?.AddListener(() => DebugUse());
        DeAssign();
        Assign();
    }

    public void Assign()
    {
        if(this.Has(out AudioPlay play))
        {
            useCallback.AddListener(play.Play);
        }
    }

    public void DeAssign()
    {
        if (this.Has(out AudioPlay play))
        {
            useCallback.RemoveListener(play.Play);
        }
    }

    
    protected override void EnterImplementationChild(Collider other)
    {
        Functionalities(other.gameObject);
    }

    protected override void StayImplementationChild(Collider other)
    {
        Functionalities(other.gameObject);
    }

    protected override void ExitImplementationChild(Collider other)
    {
        Functionalities(other.gameObject);
    }


    void Functionalities(GameObject other)
    {
        //if (other == null) return;
        if (!other.gameObject.GetComponent<Interactor>()) return;
        Interactor interactor = other.gameObject.GetComponent<Interactor>();
        interactor.ClearAllInteractEvent();

        if (singleCbCheck())
        {
            if (!holdingUseDetector)
                interactor.SetInteractEvent(callback: useCallback);
            else
            {
                interactor.SetInteracHoldEvent(callback: useCallback);
                interactor.SetInteractHoldCancledEvent(callback: holdCancledCallback);
            }
            return;
        }
        if (toggleCbCheck())
        {
            if(!holdingUseDetector)
                interactor.SetInteractEvent(toggleUseCallback[currCallback]);
            else
            {
                interactor.SetInteracHoldEvent(toggleUseCallback[currCallback]);
                interactor.SetInteractHoldCancledEvent(toggleUseCancledCallback[currCallback]);
            }
            return;
        }
    }


    public void ToggleCallback()
    {
        currCallback++;
        if (currCallback >= 2) currCallback = 0;
    }

    public override void OnRaycastedEnter(GameObject caster)
    {
        Functionalities((caster));
        base.OnRaycastedEnter (caster);
    }

    public override void OnRaycastedStay(GameObject caster)
    {
        Functionalities((caster));
        base.OnRaycastedStay(caster);
    }


    public override void OnRaycastedExit(GameObject caster)
    {
        Functionalities((caster));
        base.OnRaycastedExit(caster);
    }

    protected override void OnDestroy()
    {
        if (obj != null || raycasted)
            if (onExit)
                Exit?.Invoke();
        base.OnDestroy();
        useCallback.RemoveAllListeners();
        toggleUseCallback?.ToList().ForEach(cb => cb.RemoveAllListeners());
        DeAssign();
    }

    void DebugUse()
    {
        this.Log("Use was called");
    }
}
