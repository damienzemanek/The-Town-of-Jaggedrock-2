using System;
using System.Linq;
using DependencyInjection;
using Extensions;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CallbackDetector))]
public class Pickup : RuntimeInjectableMonoBehaviour, ICallbackUser
{
    [Inject] Interactor interactor;
    [Inject] Inventory inv;
    [field: SerializeReference] public Item presetItem;
    [field: SerializeReference] public Item item { get; private set; }

    public UnityEventPlus pickedUpEvent;
    CallbackDetector cbDetector;

    public Action<Inventory, Item> assignBindings;

    protected override void OnInstantiate()
    {
        base.OnInstantiate();
        cbDetector = GetComponent<CallbackDetector>();
        if (cbDetector == null) { Debug.LogError("Pickup: CallbackDetector is missing"); return; }
        if (presetItem == null) { Debug.LogError("Pickup: PresetItem is missing"); return; }
        if (interactor == null) { Debug.LogError("Pickup: Interactor is missing"); return; }


        item = ScriptableObject.CreateInstance<Item>();
        //print(presetItem.type);
        item.type = presetItem.type;
        item.functionality = presetItem.functionality?.Clone();
        item.icon = presetItem.icon;
        item.functionality.variations?.ForEach(v => v.Reset());
        item.canPhysicallyHold = presetItem.canPhysicallyHold;
        item.itemObj = presetItem.itemObj;

        gameObject.layer = 7;
        AssignValuesForCallbackDetector();
        pickedUpEvent.canCall = inv.IsInventoryNotFull; 
    }

    public void PickedUp(Inventory inv)
    {
        if (inv.IsInventoryFull()) return;
        print($"Pickuped up item {item.type}");

        //Uses Applying References
        item.functionality.variations?.OfType<Uses>()
            .ToList()
            .ForEach(u => u.inv = inv);

        assignBindings?.Invoke(inv, item);

        pickedUpEvent?.InvokeWithCondition(mono: this);
        this.TryGet<MeshRenderer>().enabled = false;
        transform.Children().ToList().ForEach(c => c.SetActive(false));
        Destroy(gameObject, 0.5f);
    }

    public void AssignValuesForCallbackDetector()
    {
        cbDetector.Stay.AddListener(() => interactor.SetInteractText("Pickup (E)"));
        cbDetector.Stay.AddListener(() => interactor.ToggleCanInteract(true));
        cbDetector.Exit.AddListener(() => interactor.ToggleCanInteract(false));
        cbDetector.useCallback.AddListener(() => interactor.ToggleCanInteract(false));
    }

}