using System;
using System.Collections.Generic;
using NUnit.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using ShowIfAttribute = Sirenix.OdinInspector.ShowIfAttribute;
using Sprite = UnityEngine.Sprite;
using UnityEngine.UI;

public class Use : MonoBehaviour
{
    public UnityEvent additionalActions;

    [SerializeReference] public List<UseAction> actions;

    public void UseActions(object data = null)
    {
        additionalActions?.Invoke();
        actions.ForEach(a => a.Execute(data));
    }

    public void UseAction(InventoryUsable.Data.Type itemType, object data = null)
    {
        additionalActions?.Invoke();
        actions[(int)itemType].Execute(data);
    }
}

[Serializable]
public abstract class UseAction
{
    public virtual void Execute() { }
    public virtual void Execute(object data) { }

}

[Serializable]
public class Toggle : UseAction
{
    public GameObject objToDisplay;
    public bool on = true;

    public override void Execute() => ToggleActive();
    public override void Execute(object obj) => ToggleActive();

    protected virtual void ToggleActive()
    {
        on = !on;
        objToDisplay.SetActive(on);
    }
}

public sealed class Polaroid : Toggle
{
    public Image polaroidPicture;
    [ReadOnly] public Sprite polaroidSprite;

    public override void Execute(object obj)
    {
        if(obj is Sprite sprite) SetPolaroid(sprite);
        ToggleActive();

    }
    void SetPolaroid(Sprite s) => polaroidPicture.sprite = polaroidSprite = s;
}



