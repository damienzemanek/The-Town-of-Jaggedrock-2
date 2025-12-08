using System;
using System.Linq;
using System.Text.RegularExpressions;
using ParadoxNotion.Design;
using UnityEngine;
using Random = UnityEngine.Random;
using Extensions;
using Sirenix.OdinInspector;

public class LocationRandomizer : MonoBehaviour
{

    [SerializeField] SO_Person person;
    public enum Locations
    {
        Park,
        Bakery,
        RundownHouse
    }
    public Locations _frequent => person ? person.frequent : Locations.Park;

    public enum Trait
    {
        Anxious,
        Oblivious,
        Worried,
        Calm,
    }
    public Trait _trait => person ? person.trait : Trait.Anxious;


    public string trait =>
        (_trait == Trait.Anxious) ? "I get anxious sometimes" :
        (_trait == Trait.Oblivious) ? "I can be quite oblivious sometimes" :
        (_trait == Trait.Worried) ? "My friends call me a worry-wort cause I worry so much" :
        (_trait == Trait.Calm) ? "Im usually a very calm person" :
        "";

    public string frequent =>
        (_frequent == Locations.Park) ? "In my free time I hang around the park" :
        (_frequent == Locations.Bakery) ? "The bakery always has some treats that I like" :
        (_frequent == Locations.RundownHouse) ? "I like visiting near that rundown old house" :
        "";

    [ShowInInspector, ReadOnly] public string[] activitiesT { get => setActivitiesT; }
    readonly string[] setActivitiesT =
    {
        "I went shopping recently",
        "I was visiting some family recently",
        "I met some new friends",
        "I watched the sunrise today",
        "I went and got some groceries yesterday",
        "I tried some new food for breakfast",
        "I went on a walk",
        "I was almost late to work",
        "I saw some people dancing in the forest",
        "I saw a faint glow in the forest",
        "I heard some whispering in the room next to me at night",
        "I took a nice shower",
        "I did my laundry",
        "I ate breakfast early",
        "I heard some lovely singing on my walk",
        "I saw some crows eating bread",
        "I went stargazing",
        "I stayed up doing some work in the evening"
    };

    [ShowInInspector, ReadOnly] public string[] activitiesC { get => setActivitiesC; }
    readonly string[] setActivitiesC =
    {
        "shopping",
        "family",
        "friends",
        "groceries",
        "food",
        "late",
        "dancing",
        "glow",
        "whispering",
        "shower",
        "laundry",
        "breakfast",
        "singing",
        "crows",
        "stargaze",
        "evening"
    };

    [ShowInInspector, ReadOnly] public string[] introspectionT { get => setIntrospectionT; }
    readonly string[] setIntrospectionT =
    {
        "I love having the bread at the bakery",
        "Mornings are the best time to get things done",
        "The traffic has been quite good lately",
        "I wonder when it's going to rain",
        "Singing and dancing is a fun passtime",
        "Sometimes I think I heard someone say something, but I know for sure noones there",
        "Do you think the library has any new books in?",
        "This hotel has pretty nice rooms",
        "Rain rain go away, come again another day!",
        "The forest looks pretty from my window.",
        "When the wind blows just right, you can smell the farm from here.",
        "The town stray cat died. Sad day",
        "There are some fancy dancers in the town!",
        "Need me some more money",
        "The trail is quiet, good for a long walk",
        "Have you been in the forest? I have. Spooky.",
        "I heard the forest's whispers.",
        "Some people are weird in this town.",
        "Someone here scares me.",
        "I never break the law. Im a good person I swear.",
        "Sometimes I keep my curtains closed, So I can stay in bed a little later",
        "Can you whistle? I can, a little melody for you..",
        "I've been suuuuper tired lately",
        "The government is putting chemicals in our food...",
        "Coffee makes my brain fuzzy.",
        "Can't stand the rain man. Gets all over my stuff.",
        "Have you heard the song \"Bridge over troubled water\"?"
    };

    [ShowInInspector, ReadOnly] public string[] introspectionC { get => setIntrospectionC; }
    readonly string[] setIntrospectionC =
    {
        "Nighttime is the best time to get things done",
        "I hope it's going to rain",
        "Singing and dancing is great for the soul",
        "Do you think the library has any new books any interesting knowledge?",
        "The forest looks magical from my window.",
        "When the wind blows just right, you can smell the soil from here.",
        "The town stray cat died.",
        "The trail is quiet, good to hear nature",
        "Have you been in the forest? I have. Sureal.",
        "I listen to the forest's whispers.",
        "I never break the law. Im a good person I swear.",
        "Sometimes I keep my curtains closed, The morning sun is a little too bright",
        "Can you whistle? I can, a little melody for you..",
        "I've been suuuuper tired lately, staying up late does that to you.",
        "Herbal tea makes my brain fuzzy.",
        "I love going out in the chilly rain",
        "Bridge.. Over... Troubled.... water..."
    };


    private void OnValidate()
    {
        if(person == null)  person = this.Get<Dialuage>().so_person;
        if(town == null) town = this.Get<Town>();
    }

    public Locations RandLocEnumExclude(params Locations[] exclude) => EnumEX<Locations>.Rand(exclude);
    public Locations RandLocEnum() => EnumEX<Locations>.Rand();

    public string activityT => activitiesT.Rand();
    public string activityC => activitiesC.Rand();
    public string introspectT => introspectionT.Rand();
    public string introspectC => introspectionC.Rand();
    public string introspect => (town == null) ? "" : 
        (town.isCoven) ? introspectC : introspectT;


    public Town town;



    //  public string RandActivity { get => }

}
