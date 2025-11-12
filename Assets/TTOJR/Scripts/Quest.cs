using System.Linq;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Events;
using static Questing.Hotel;
using static Questing.Town;
using static Questing.Activity;
using Extensions;
using NUnit.Framework;
using System.Collections.Generic;
using Sirenix.OdinInspector;


public static class Questing
{
    public enum Section
    {
        None,
        TOWN,
        HOTEL,
        ACTIVITY,
    }
    public abstract class QuestType
    {
        public abstract Enum quest { get; set; }
    }
    public abstract class QuestType<TEnum> : QuestType where TEnum : Enum
    {
        public TEnum TypedQuest;
        public override Enum quest
        {
            get => TypedQuest;
            set => TypedQuest = value is TEnum e ? e : default;
        }

        public QuestType(TEnum typedQuest)
        {
           TypedQuest = typedQuest;
        }
    }


    [Serializable]
    public class Town : QuestType<Town.Quest>
    {
        public enum Quest
        {
            None,
            MANIA_OF_INJUSTICE,
            CORRUPTED_ROOTS,
            SACRIFICIAL_LAMBS
        }

        public static int size { get => EnumEX<Quest>.Size(); }
        public static int[] progressionLengths = { 0, 3, 3, 3 };
        public Town() : base(default) { }
        public Town(Quest q) : base(q) { }
    }

    [Serializable]
    public class Hotel : QuestType<Hotel.Quest>
    {
        public enum Quest
        {
            None,
            THE_DEVILS_NUMBER,
            BLOOD_IN_THE_WATER,
            SOLUS_IMMUNIS
        }

        public static int size { get => EnumEX<Quest>.Size(); }
        public static int[] progressionLengths = { 0, 3, 3, 3 };

        public Hotel() : base(default){ }
        public Hotel(Quest q) : base(q) { }
    }

    [Serializable]
    public class Activity : QuestType<Activity.Quest>
    {
        public enum Quest
        {
            None,
            CLEANROOM,
            REPAIRELECTRICITY,
        }
        public static int size { get => EnumEX<Quest>.Size(); }
        public static int[] progressionLengths = { 0, 3, 3, 3 };

        public Activity() : base(default) { }
        public Activity(Quest q) : base(q) { }
    }


}



[Serializable]
public class Quest
{
    #region Privates

    #endregion

    [Serializable]
    public class ProgressionEvent
    {
        public bool completed;
        public UnityEvent completeEvent;
        public ProgressionEvent()
        {
            completed = false;
        }

        public void Complete()
        {
            this.Log("Completed Progression Event");
            completeEvent?.Invoke();
            completed = true;
        }
    
    }

    [PropertyOrder(0)] public bool active = false;
    [PropertyOrder(1)] [field: ShowInInlineEditors] [ShowInInspector]
    public int currentProggressLevel
    {
        get
        {
            int index = Array.FindLastIndex(progression, p => p.completed == true);
            return (index == -1) ? 0 : index;
        }
    }
    [PropertyOrder(2)] public Questing.Section section = Questing.Section.None;
    [PropertyOrder(3)][SerializeReference] public Questing.QuestType type;

    [PropertyOrder(4)][SerializeReference] public ProgressionEvent[] progression;

    public class QuestBuilder
    {
        Quest quest;
        public QuestBuilder() => quest = new Quest();
        public static QuestBuilder Start() { return new QuestBuilder(); }

        public QuestBuilder WithSection(Questing.Section s)
        {
            quest.active = false;
            quest.section = s;
            return this;
        }

        public QuestBuilder WithTownQuest(Questing.Town.Quest q)
        {
            quest.type = new Questing.Town(q);
            return this;
        }

        public QuestBuilder WithHotelQuest(Questing.Hotel.Quest q)
        {
            quest.type = new Questing.Hotel(q);
            return this;
        }

        public QuestBuilder WithActivity(Questing.Activity.Quest q)
        {
            quest.type = new Questing.Activity(q);
            return this;
        }

        public QuestBuilder WithProgression(ProgressionEvent[] _progression)
        {
            quest.progression = _progression;
            return this;
        }
        public Quest Build() => quest;

    }

    public static T GetRandomQuest<T>(out int index) where T : Enum
    {
        Array values = Enum.GetValues(enumType: typeof(T));
        index = Random.Range(0, maxExclusive: values.Length);
        T type = (T)values.GetValue(index);
        return type;
    }

    public bool questComplete => (progression != null) ? 
        progression.Last().completed == true : 
        false;


    public void Activate() => active = true;


    #region Methods

    public static Quest CreateNewQuest<TEnum>(Questing.Section s, Enum enumType, List<Quest> quests = null)
    {
        int length = 0;
        Quest quest = null;

        //Hotel Quest
        if(enumType is Questing.Hotel.Quest hotel)
        {
            length = Questing.Hotel.progressionLengths[(int)hotel];

            ProgressionEvent[] progression = new ProgressionEvent[length]
                .Populate(() => new ProgressionEvent());

            quest = QuestBuilder.Start()
                .WithSection(s)
                .WithHotelQuest(hotel)
                .WithProgression(progression)
                .Build();
        }

        //Town Quest
        if (enumType is Questing.Town.Quest town)
        {
            length = Questing.Town.progressionLengths[(int)town];

            ProgressionEvent[] progression = new ProgressionEvent[length]
                .Populate(() => new ProgressionEvent());

            quest = QuestBuilder.Start()
                .WithSection(s)
                .WithTownQuest(town)
                .WithProgression(progression)
                .Build();
        }

        //Activity
        if (enumType is Questing.Activity.Quest activity)
        {
            length = Questing.Activity.progressionLengths[(int)activity];

            ProgressionEvent[] progression = new ProgressionEvent[length]
                .Populate(() => new ProgressionEvent());

            quest = QuestBuilder.Start()
                .WithSection(s)
                .WithActivity(activity)
                .WithProgression(progression)
                .Build();

        }

        quests?.Add(quest);
        return quest;
    }

    #endregion
}




