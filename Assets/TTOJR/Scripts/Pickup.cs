using System;
using System.Linq;
using DependencyInjection;
using Extensions;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(1)]
[RequireComponent(typeof(CallbackDetector))]
public class Pickup : RuntimeInjectableMonoBehaviour, ICallbackUser
{
    [Inject] Interactor interactor;
    [Inject] Inventory inv;
    [field: SerializeReference] public Item presetItem;
    [field: SerializeReference] public Item item { get; private set; }

    public UnityEventPlus pickedUpEvent;
    CallbackDetector cbDetector;

    public Action<Inventory, Item> physicalUseHookPoint;

    protected override void OnInstantiate()
    {
        base.OnInstantiate();
        cbDetector = GetComponent<CallbackDetector>();
        if (cbDetector == null) { Debug.LogError("Pickup: CallbackDetector is missing"); return; }
        if (presetItem == null) { Debug.LogError("Pickup: PresetItem is missing"); return; }
        if (interactor == null) { Debug.LogError("Pickup: Interactor is missing"); return; }


        CopyOverItem();

        gameObject.layer = 7;
        AssignValuesForCallbackDetector("Pickup (E)");
        pickedUpEvent.canCall = inv.IsInventoryNotFull;

        void CopyOverItem()
        {
            //New instance of the item SO

            //Copy over paramters
            item = ScriptableObject.CreateInstance<Item>(); 
            item.type = presetItem.type;
            item.functionality = presetItem.functionality?.Clone();
            item.icon = presetItem.icon;
            item.itemObj = presetItem.itemObj;
            item.canPhysicallyHold = presetItem.canPhysicallyHold;
            item.selectSFX = presetItem.selectSFX;
            item.pickupSFX = presetItem.pickupSFX;

            //Reset the variations (currently used in Uses to reset the used amount)
            item.functionality.variations?.ForEach(v => v.Reset());
        }
    }

    public void PickedUp(Inventory inv)
    {
        if (inv.full) return;
        print($"Pickuped up item {item.type}");

        //Setting dependancies for variations
        // Some variants depend on: Inventory, Interactor
        item.functionality.variations?.ForEach(v => v.WithDependancies(inv, interactor));

        //
        physicalUseHookPoint?.Invoke(inv, item);

        if(item.pickupSFX != null) inv.TryGetOrAdd<AudioSource>().Play(item.pickupSFX);

        pickedUpEvent?.InvokeWithCondition(mono: this);
        if (this.Has(out MeshRenderer m)) m.enabled = false;
        transform.Children().ToList().ForEach(c => c.SetActive(false));
        Destroy(gameObject);
        if (inv.IsInventoryFull()) inv.full = true;
        else inv.full = false;
    }

    public void AssignValuesForCallbackDetector(string interactText)
    {
        cbDetector.onStay = true;
        cbDetector.Stay.AddListener(() => interactor.SetInteractText(interactText));
        cbDetector.Stay.AddListener(() => interactor.ToggleCanInteract(true));
        cbDetector.Exit.AddListener(() => interactor.ToggleCanInteract(false));
        cbDetector.useCallback.AddListener(() => interactor.ToggleCanInteract(false));
    }

    private void OnDestroy()
    {
        cbDetector.Stay.RemoveAllListeners();
        cbDetector.Exit.RemoveAllListeners();
        cbDetector.useCallback.RemoveAllListeners();
    }



}