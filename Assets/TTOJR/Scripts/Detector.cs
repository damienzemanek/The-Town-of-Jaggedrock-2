using System.ComponentModel;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using ReadOnlyAttribute = Sirenix.OdinInspector.ReadOnlyAttribute;
using Extensions;

public class Detector : MonoBehaviour
{
    [Title("Detector")]
    [PropertySpace(spaceBefore: 1)]
    [field:InfoBox("Who can interact with me")] [field: SerializeField] public LayerMask locationMask { get; private set; }
    [FoldoutGroup("Detector Type")] public bool rayCastDetector;
    [FoldoutGroup("Detector Type")] public bool collisionDetector;

    bool notCaster => !rayCastDetector;
    [FoldoutGroup("Runtime")][SerializeField][ReadOnly]
    protected GameObject obj;
    [FoldoutGroup("Runtime")][ShowIf("rayCastDetector")][ReadOnly] 
    public bool raycasted;
    [FoldoutGroup("Runtime")][SerializeField][ReadOnly] 
    protected bool somethingCollided;
    [FoldoutGroup("Runtime")] [SerializeField][ReadOnly]
    protected bool raycastEntered = false;


    [HideInInspector] public GameObject casterObject { get => obj; set => obj = value; }
    [HideInInspector] public GameObject colliderObject { get => obj; set => obj = value; }

    [PropertyOrder(0)][FoldoutGroup("Enable Unity Events")][SerializeField] public bool onEnter;
    [PropertyOrder(1)][FoldoutGroup("Enable Unity Events")][ShowIf("onEnter")] public UnityEvent Enter;
    [PropertyOrder(2)][FoldoutGroup("Enable Unity Events")][SerializeField] public bool onStay;
    [PropertyOrder(3)][FoldoutGroup("Enable Unity Events")][ShowIf("onStay")] public UnityEvent Stay;
    [PropertyOrder(4)][FoldoutGroup("Enable Unity Events")][SerializeField] public bool onExit;
    [PropertyOrder(5)][FoldoutGroup("Enable Unity Events")][ShowIf("onExit")] public UnityEvent Exit;
    [PropertySpace(spaceBefore: 2)]

    public Detector Init(bool? enter = null, bool? stay = null, bool? exit = null)
    {
        onEnter = enter ?? false;
        onStay = stay ?? false;
        onExit = exit ?? false;
        return this;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!collisionDetector) return;
        if (!onEnter) return;
        if (!IsInLayer(other)) return;
        obj = other.gameObject;
        this.Log("Trigger ENTER");
        EnterImplementationChild(other);
        Enter?.Invoke();
    }
    protected virtual void EnterImplementationChild(Collider other) { }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (!collisionDetector) return;
        if (!IsInLayer(other)) return;
        somethingCollided = true;
        if (!onStay) return;
        obj = other.gameObject;
        this.Log("Trigger STAY");
        StayImplementationChild(other);
        Stay?.Invoke();
    }
    protected virtual void StayImplementationChild(Collider other) { }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!collisionDetector) return;
        if (!onExit) return;
        if (!IsInLayer(other)) return;
        this.Log("Trigger EXIT");
        ExitImplementationChild(other);
        Exit?.Invoke();
        obj = null;
        somethingCollided = false;
        
    }
    protected virtual void ExitImplementationChild(Collider other) { }


    protected bool IsInLayer(Collider other)
    {
        if ((locationMask.value & (1 << other.gameObject.layer)) == 0) return false;
        else return true;
    }

    protected bool CasterInLayer(GameObject other)
    {
        if ((locationMask.value & (1 << other.gameObject.layer)) == 0) return false;
        else return true;
    }

    GameObject casterBuffer = null;
    public virtual void OnRaycastedEnter(GameObject caster)
    {
        if (!rayCastDetector) return;
        if (!onEnter) return;
        if (!CasterInLayer(caster)) return;
        if (raycastEntered) return;
        raycastEntered = true;
        obj = caster.gameObject;
        casterBuffer = caster;
        raycasted = true;

        Enter?.Invoke();
        this.Log("Raycast ENTER");
    }

    public virtual void OnRaycastedStay(GameObject caster)
    {
        if (!rayCastDetector) return;
        if (!raycastEntered) OnRaycastedEnter(caster);
        CancelInvoke(nameof(DisableRaycasted));
        Invoke(nameof(DisableRaycasted), 0.1f);


        if (!onStay) return;
        if (!CasterInLayer(other: caster)) return;
        this.Log("Raycast STAY");
        obj = caster.gameObject;
        casterBuffer = caster;
        raycasted = true;

        Stay?.Invoke();
    }


    public virtual void OnRaycastedExit(GameObject caster)
    {
        this.Log("Attempting to raycast exit");
        if (!rayCastDetector) return;
        if (!onExit) return;
        this.Log("Raycast EXIT");
        Exit?.Invoke();
        obj = null;
        raycasted = false;
        raycastEntered = false;
    }

    void DisableRaycasted()
    {
        this.Log($"Disabling raycast with casterbuffer {casterBuffer}");
        OnRaycastedExit(caster: casterBuffer);
    }

    protected virtual void OnDestroy()
    {
        Enter.RemoveAllListeners();
        Stay.RemoveAllListeners();
        Exit.RemoveAllListeners();
    }

}
