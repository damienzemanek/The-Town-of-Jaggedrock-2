using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Extensions;
using Unity.VisualScripting;

[Serializable]
public class ActionChoices
{
    public List<WeightedAction> actions;

    public void DoAnAction(NPC_Area area)
    {
        WeightedAction wa = DetermineActionToExecute();
        wa.action.Execute(area);

        this.Log($"Executing action: {wa.action.GetType().ToString()}");
        
    }

    public WeightedAction DetermineActionToExecute()
    {
        this.Log("Determining Which action to execute");
        this.Log("chosen actions count: " + actions.Count);

        if (actions == null || actions.Count == 0) return null; //No actions

        var validActions = actions.Where(a => a.chance > 0).ToList();
        if (validActions.Count == 0) this.Warn("No valid actions with a chance of > 0");
        
        this.Log("chosen valid actions count " + validActions.Count);

        List<WeightedAction> pool = new List<WeightedAction>();

        foreach (var action in validActions)
            for (int i = 0; i < action.chance; i++)
                pool.Add(item: action);

        this.Log("chosen pool lenght: " + pool.Count);
        pool.ForEach(p => this.Log("Chosen out of" + p.action.GetType()));

        WeightedAction chosen = pool.Rand();
        this.Log($"Chosen {chosen.action.GetType()}");

        return chosen;
    }


}
