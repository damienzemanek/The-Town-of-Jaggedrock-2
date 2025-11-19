using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Sirenix.OdinInspector;
using DependencyInjection;
using Extensions;
using UnityEngine.Events;
using System.Linq;
using Sirenix.Utilities;

public class NPC_Spawner : MonoBehaviour, IDependencyProvider
{
    [Provide] NPC_Spawner Proviide() => this;
    [Inject] TimeCycle time;
    [SerializeField] int day;
    [SerializeField] int night;
    [SerializeField] public List<GameObject> npcs;
    public struct SpawnData
    {
        public bool[] spawn;
    }


    public List<SpawnData> daySpawns;
    public List<SpawnData> nightSpawns;

    private void OnEnable()
    {
        time.OnDayStart.AddListener(SpawnDay);
        time.OnNightStart.AddListener(SpawnNight);
    }

    private void OnDisable()
    {
        time.OnDayStart.RemoveListener(SpawnDay);
        time.OnNightStart.AddListener(SpawnNight);
    }



    public void SpawnDay()
    {
        foreach (var active in daySpawns[day].spawn)
            npcs.ForEach(npc => npc.SetActive(active));

    }

    public void SpawnNight()
    {
        foreach (var active in nightSpawns[night].spawn)
            npcs.ForEach(npc => npc.SetActive(active));

    }




}
