using System;
using System.Linq;
using System.Text.RegularExpressions;
using ParadoxNotion.Design;
using UnityEngine;
using Random = UnityEngine.Random;
using Extensions;

public class LocationRandomizer : MonoBehaviour
{
    public enum Locations
    {
        Hotel,
        Sherriffs,
        Park,
        Bakery,
        Store,
        Diner,
        Library,
        Courthouse,
        Mansion,
        Offices,
        Trail,
        Farm,
        Forest,
        RundownHouse
    }

    public string[] locations; //Display Names
    public static Locations[] frequentLocations; //Display Names


    public string[] activities =
    {
        "went shopping",
        "visited some family",
        "met some new friends",
        "watched the sunrise",
        "got some groceries",
        "tried some new food",
        "went on a walk",
        "was almost late to work",
        "saw some people dancing in the forest",
        "saw a faint glow in the forest",
        "heard some whispering in the room next to me at night",
        "took a nice shower",
        "did my laundry",
        "ate breakfast",
        "heard some lovely singing on my walk",
        "saw some crows eating bread",
        "went stargazing",
        "stayed up late doing some work"
    };


    private void Awake()
    {
        if (frequentLocations == null || frequentLocations.Length == 0)
            frequentLocations = new Locations[]
            {
                RandLocEnumExclude(Locations.Hotel),
                RandLocEnumExclude(Locations.Hotel),
                RandLocEnumExclude(Locations.Hotel),
                RandLocEnumExclude(Locations.Hotel)
            };
        SetLocs();




    }
    private void OnValidate() => SetLocs();
    static string ConvertLocEnumToFormattedString(Locations loc) =>
        Regex.Replace(loc.ToString(), "([a-z])([A-Z])", "$1 $2");

    void SetLocs() => locations = Enum.GetValues(typeof(Locations))
                        .Cast<Locations>()
                        .Select(ConvertLocEnumToFormattedString)
                        .ToArray();

    public string RandLoc { get => locations.Rand(); }
    public string RandLocExcludeHotel => locations[(int)RandLocEnumExclude(Locations.Hotel)];
    public Locations RandLocEnumExclude(params Locations[] exclude) => EnumEX<Locations>.Rand(exclude);
    public Locations RandLocEnum() => EnumEX<Locations>.Rand();
    public string RandAct => activities.Rand();



    //  public string RandActivity { get => }

}
