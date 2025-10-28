using System;
using DependencyInjection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using Extensions;

public class InventorySlot : RuntimeInjectableMonoBehaviour
{
    [field: SerializeField] public Image displayIcon { get; private set; }
    [field: SerializeField] public GameObject selectedIndicator { get; private set; }
    [field: SerializeField] public Item myItem { get; private set; }

    [Inject] Sprite emptyIcon;
    protected override void Awake()
    {
        base.Awake();
        displayIcon = GetComponent<Image>();
    }
    public void Init()
    {
        selectedIndicator.SetActive(false);
    }

    public void SetSlot(Item item)
    {
        this.Log($"Setting Slot with item {item.type.ToString()}");
        myItem = item;
        displayIcon.sprite = item.icon;
    }

    public void ResetSlot()
    {
        myItem = null;
        displayIcon.sprite = emptyIcon;
    }

    public void Select()
    {
        selectedIndicator.SetActive(true);
    }

    public void Unselect()
    {
        selectedIndicator.SetActive(false);

    }

}

