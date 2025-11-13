using System.Collections.Generic;
using System.Linq;
using Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
public class InventoryUpdater : MonoBehaviour
{
    #region Privates
    PreRequisiteCallbackDetector detector;
    [SerializeField] bool variationDataUpdate;
    [SerializeField] bool functionalityDataUpdate;
    [ShowIf("functionalityDataUpdate")] [SerializeField] UnityEvent functionalityUseItemCallback;
    [InlineEditor][field: SerializeReference] List<Item> itemDataPhases;
    #endregion

    private void Start()
    {
        if (detector == null) detector = this.Get<PreRequisiteCallbackDetector>();
        if (itemDataPhases == null) this.Error("Item data phases not set");
        if (!detector.lookingForChangesToItem) this.Error("Prereq detector looking for changes to item is null");


        itemDataPhases.Where(i => i.type != detector.lookingForChangesToItem.type)?
            .ToList()?
            .ForEach(notSameType => Debug.LogError(
            $"InventoryUpdater: ({gameObject.name})'s item {notSameType.name} type ({notSameType.type}) does not match detector’s expected type ({detector.lookingForChangesToItem.type})"
        ));
    }

    [Button]
    public void AddItemDataPhase(Item item)
    {
        if(item != null)
            itemDataPhases.Add(item.Clone());
    }


    // An item's data can be updated 2 ways
    // 1 Through their functionality
    // 2 Through their variations
    public void UpdateItem(int phase)
    {
        //Items can't be updates if the detector isnt detection an interaction
        if (detector.casterObject == null) Debug.LogError("Inv: UPDATER detector obj is NULL");

        // First, get the item data on the specified phase (index),
        // Second, Get the detected object's inventory
        // Third, Find an item in the player's inventory that is equal to the type of the interact item
        Item interactItem = itemDataPhases[phase];
        Inventory inv = detector.casterObject.gameObject.GetComponent<Inventory>();
        Item invItem = inv.pickedUpItems.FirstOrDefault(itm => itm != null && itm.type == interactItem.type);
        if(invItem == null) {  print("Inv: UPDATER no item found of that type, returning"); return; }


        print($"Inv: UPDATER attempting item: {invItem.type.ToString()} update phase {phase}");


        //Updating inv item's FUNCTIONALITY data with the data from the interact item's FUNCTIONALITY data
        if (functionalityDataUpdate)
            invItem.functionality.UpdateFunctionalityData(interactItem.functionality.Data);

        //Updating inv item's VARIATION data with the data from the interact item's VARIATION data
        if (variationDataUpdate)
            VariationDataUpdate(invItem, interactItem.functionality);

    }

    public void VariationDataUpdate(Item invItem, IItemFunctionality interactItemFunctionality)
    {
        for (int invVariant = 0; invVariant < invItem.functionality.variations.Count; invVariant++)
        {
            for (int interactVariant = 0; interactVariant < interactItemFunctionality.variations.Count; interactVariant++)
            {
                //For every invItem Variation, check it against every interactItemVariation
                //If its not a match keep searching
                if (invItem.functionality.variations[invVariant].GetType() != interactItemFunctionality.variations[interactVariant].GetType())
                    continue;

                //if its a match, Update the value of the (Inv) to the value of the (interact)
                invItem.functionality.variations[invVariant].UpdateData(interactItemFunctionality.variations[interactVariant]);
            }
        }
    }

    public void UseItem()
    {
        Inventory inv = detector.casterObject.gameObject.GetComponent<Inventory>();
        print(inv);
        Item invItem = inv.GetCurrentItem();

        if (invItem == null) return;
        if (invItem.functionality.GetType() != detector.lookingForChangesToItem.functionality.GetType())
        {
            Debug.LogWarning($"InventoryUpdater: Trying to use Item that is NOT the same type" +
            $" ({invItem.functionality.GetType()}) and ({detector.lookingForChangesToItem.functionality.GetType()})"); 
        }

        print($"Inventory: UPDATER using item {invItem.type.ToString()} which is a {invItem.functionality.GetType()}");
        invItem.functionality.UseIfVariantsAllow(functionalityUseItemCallback);
    }

}
