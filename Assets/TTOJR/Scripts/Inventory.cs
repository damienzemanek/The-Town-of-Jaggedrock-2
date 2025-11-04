using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Interactor))]
public class Inventory : MonoBehaviour, IDependencyProvider
{
    public const int INV_SIZE = 5;
    [Inject] MainCamera cam;
    [BoxGroup(group: "Runtime")][field: SerializeReference] public Item[] pickedUpItems { get; set; } = new Item[INV_SIZE];

    [BoxGroup(group: "Runtime")][SerializeField] int selectItem;
    [BoxGroup(group: "Runtime")][SerializeField] Pickup potentialItem;
    [BoxGroup(group: "Runtime")][SerializeField] public bool canPickup { get; private set; }

    [Inject] EntityControls controls;
    public Interactor interactor { get; private set; }
    [BoxGroup(group: "UI")][field: SerializeField] public GameObject gridParent { get; private set; }
    [BoxGroup(group: "UI")][field: SerializeField] public GameObject inventorySlotPrefab { get; private set; }
    [BoxGroup(group: "UI")][field: SerializeField] public Sprite emptyIcon { get; private set; }
    [Provide] public Sprite ProvideEmptyIcon() => emptyIcon;
    [Provide] Inventory Provide() => this;

    [BoxGroup(group: "UI")][SerializeField] int slotCount;

    private void Awake()
    {
        interactor = GetComponent<Interactor>();
    }

    private void OnEnable()
    {
        controls.interact += Interact;

        for (int i = 0; i < EntityControls.INVENTORY_NUMS; i++)
        {
            controls.intentoryNums[i] += SelectItem;
        }

        interactor.RaycasterEvent += PickupRaycast;
        interactor.FailedRaycast += PickupRaycastFailed;
    }

    private void OnDisable()
    {
        controls.interact -= Interact;

        interactor.RaycasterEvent -= PickupRaycast;
        interactor.FailedRaycast -= PickupRaycastFailed;
    }

    private void Start()
    {
        CreateInventorySlots();
    }

    void PickupRaycast(Ray ray, RaycastHit hit)
    {
        if (!hit.transform.gameObject.GetComponent<Pickup>()) return;
        if (IsInventoryFull()) return;
        //print("Pickup raycast");
        Debug.DrawLine(ray.origin, hit.point, Color.green);
        TogglePickup(true);
        potentialItem = hit.transform.gameObject.GetComponent<Pickup>();
    }

    void PickupRaycastFailed()
    {
        TogglePickup(false);
    }

    void TogglePickup(bool val)
    {
        //print($"toggling pickup {val}");
        potentialItem = null;
        canPickup = val;
    }

    void Interact()
    {
        print("Inv: Interacting");
        if (canPickup)
            Pickup();
    }

    void Pickup()
    {
        Item newItem = potentialItem.item;
        int newIndex = 0;
        for (int i = 0; i < pickedUpItems.Length; i++)
        {
            if (pickedUpItems[i] == null)
            {
                pickedUpItems[i] = newItem;
                newIndex = i;
                break;
            }
        };
        print($"Inv: new item is {newItem.type.ToString()}");
        SetDisplayItem(newItem, newIndex);
        SelectItem(newIndex);

        object newData = newItem.functionality.Data;
        if (newData == null) Debug.LogError("Item SO functionality not set (null)");

        potentialItem.PickedUp(this);
    }

    void CreateInventorySlots()
    {
        for(int i = 0; i < slotCount; i++)
        {
            InventorySlot slot = Instantiate(inventorySlotPrefab, gridParent.transform.position, Quaternion.identity, gridParent.transform)
                .GetComponent<InventorySlot>();
            slot.Init();
        }
    }

    void SetDisplayItem(Item item, int i)
    {
        print("Inv: Setting new display item");
        gridParent.transform.GetChild(i).GetComponent<InventorySlot>().SetSlot(item);
    }
    
    void DisplayItem(int i)
    {
        gridParent.transform.GetChild(i).GetComponent<InventorySlot>().Select();
        selectItem = i;
    }

    void SelectItem(int num)
    {
        print($"Inv: Selecting Item index {num}");
        print("Inv: Selecting Item index {num}");


        UnselectAllItems();
        DisplayItem(num);

        //Guard Clauses (for not having an item in the slot)
        if (num >= pickedUpItems.Length) return; if (pickedUpItems.Length <= 0) return;
        if (pickedUpItems[num] == null)
            print($"Inv: Item selected is NULL or EMPTY");
        else
            print($"Inv: Item selected is {pickedUpItems[num].type.ToString()}");
        
        //Enable the Item's object if the player can hold the item
        TryToEnableItemObject(num, pickedUpItems[num]);

        //Set all locational PreReqs to either (T or F) based on this item.
        PreRequisiteCallbackDetector.hasItemPreRequisite.Invoke(pickedUpItems[num], true);



        void TryToEnableItemObject(int num, Item item)
        {
            ItemHolder holder = GetComponent<ItemHolder>() ?? throw new Exception("Inv: No Item holder found");
            holder.DisableAllObjects();
            if (item == null) return;
            if (!item.canPhysicallyHold) return;
            holder.UseItem(num, item);
        }

    }

    void UnselectAllItems()
    {
        print("Inv: Unsellecting all items");
        gridParent.GetComponentsInChildren<InventorySlot>(true)
        .ToList()
        .ForEach(action: s => s.Unselect());
        PreRequisiteCallbackDetector.HasItemPrequisitesReset();
    }

    public void RemoveItem(Item item)
    {
        for(int i = 0; i < pickedUpItems.Length; i++)
            if (pickedUpItems[i] == item)
                pickedUpItems[i] = null;
    }

    public void RemoveCurrentSelectedItem()
    {
        PreRequisiteCallbackDetector.hasItemPreRequisite?.Invoke(pickedUpItems[selectItem], false);
        RemoveItem(pickedUpItems[selectItem]);
        gridParent.transform.GetChild(selectItem).GetComponent<InventorySlot>().ResetSlot();
        interactor.FailedRaycast?.Invoke();
        interactor.InteractEvent = null;
    }

    public Item GetCurrentItem()
    {
        if (pickedUpItems.Length <= 0) return null;
        if (pickedUpItems.Length <= selectItem) return null;
        if (pickedUpItems[selectItem] != null) 
            return pickedUpItems[selectItem];

        return null;
    }

    public void ToggleInventoryVisability(bool val) => gridParent?.SetActive(val);

    public bool IsInventoryFull() => pickedUpItems.All(i => i != null);
    public bool IsInventoryNotFull() => pickedUpItems.Any(i => i == null);

}
