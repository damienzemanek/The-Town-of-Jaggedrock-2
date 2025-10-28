using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection;
using Extensions;
using Sirenix.OdinInspector;
using Unity.AI.Navigation.Samples;
using UnityEngine;
using static Questing;

public abstract class Questholder<TQuestEnum> : RuntimeInjectableMonoBehaviour where TQuestEnum : Enum
{
    public abstract Section Section { get; set; }
    [field:SerializeReference] public List<Quest> quests { get; set; }

    [Button]
    void CreateNewQuest(Enum enumType) 
        => Quest.CreateNewQuest<QuestType>(Section, enumType, quests);

    public bool hasAnActiveQuest => (quests.Any(q => q.active));
    public Quest currentQuestReferece => (hasAnActiveQuest) ? quests.FirstOrDefault(q => q.active) : null;

    [Button]
    protected TQuestEnum SyncCurrentQuest()
    {
        var q = quests.FirstOrDefault(q => q.active);

        return (TQuestEnum)q.type.quest;
    }

    public abstract TQuestEnum currentQuest { get; set; }


    public int progressLevelOfCurrentQuest => currentQuestReferece?.currentProggressLevel ?? 0;
    [Button]
    public void ActivateNewRandomQuest()
    {
       quests?.Rand()?.Activate();
       currentQuest = SyncCurrentQuest();
    }

    public void StartQuestProgress() => currentQuestReferece.progression[0].Complete();

    public void IncreaseProgressionOfCurrentQuest() => currentQuestReferece.progression.FirstOrDefault(p => p.completed == false).Complete();

}
