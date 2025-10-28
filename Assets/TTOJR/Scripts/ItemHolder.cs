using System.Linq;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    public Transform itemParent;
    public ItemSlot[] slots = new ItemSlot[5];
    public GameObject currentItem;

    private void Awake()
    {
        if (itemParent.transform.childCount > 0) Debug.LogError("Item Holder Parent" +
            "Has children, delete them before starting");


        while (itemParent.transform.childCount < slots.Length)
        {
            GameObject newGo = Instantiate(new GameObject(), itemParent.transform);
            newGo.AddComponent<ItemSlot>();
        }

        for (int i = 0; i < slots.Length; i++)
            slots[i] = itemParent.transform.GetChild(i).gameObject.GetComponent<ItemSlot>();

        DisableAllObjects();
    }

    public void DisableAllObjects()
    {
        slots.ToList().ForEach(s =>
        {
            if (s.transform.childCount > 0)
                s.transform.GetChild(0).gameObject.SetActive(false);
        });

    }

    public void UseItem(int num, Item item)
    {
        if (slots[num].transform.childCount <= 0)
        {
            if (item == null) throw new System.Exception("No Item found");
            if (item.itemObj == null) throw new System.Exception("Item's itemObj is not set or null");

            var go = Instantiate(item.itemObj, slots[num].transform);
        }
        else
        {
            slots[num].transform.GetChild(0).gameObject.SetActive(true);
        }

        currentItem = slots[num].transform.GetChild(0).gameObject;
    }

}
