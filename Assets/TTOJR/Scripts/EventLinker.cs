using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection;
using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;
using Extensions;

public class EventLinker : MonoBehaviour
{

    #region Privates

    //Needed ai help with this one to see how to narrow the dropdowns search
    private static IEnumerable<ValueDropdownItem<Type>> QuestEventTypesDropdown()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && typeof(QuestEventBase).IsAssignableFrom(t))
            .OrderBy(t => t.Name)
            .Select(t => new ValueDropdownItem<Type>(t.Name, t));
    }

    #endregion

    [SerializeReference] public List<QuestEventBase> events;

    public void ExecuteAllEvents() => events?.ForEach(e => e.Execute());

    public void ExecuteEvent(int index)
    {
        events[index]?.Execute();
    }

    [Button]
    public void AddEvent([ValueDropdown(nameof(QuestEventTypesDropdown))] Type eventType)
    {
        if (eventType == null) return;

        GameObject newEventGO = Instantiate(new GameObject());
        newEventGO.transform.SetParent(transform, false);
        newEventGO.name = $"EVENT: {eventType}";

        Component newEvent = newEventGO.AddComponent(eventType);
        events.Add((QuestEventBase)newEvent);
    } 

    public void AddEvent<T>(T eventType) where T : QuestEventBase
    {
        GameObject newEventGO = Instantiate(new GameObject());
        newEventGO.transform.SetParent(transform, false);
        newEventGO.name = $"EVENT: {eventType}";
       
        Component newEvent = newEventGO.AddComponent<T>();
        events.Add((QuestEventBase)newEvent);
    }


    #region Methods

    #endregion

}