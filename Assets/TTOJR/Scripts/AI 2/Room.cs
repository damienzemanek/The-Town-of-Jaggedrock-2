using System.Collections.Generic;
using System.Linq;
using DependencyInjection;
using Extensions;
using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Inject] TimeCycle time;

    [SerializeField] public int roomNum;
    [SerializeField] public NPC_Area tpBackArea;
    [SerializeField] public NPC_Area outsideRoomArea;

    #region Privates
    [ShowInInspector, ReadOnly] Teleport tp;
    [SerializeField, ReadOnly] Town resident;
    #endregion



    private void Awake()
    {
        tp = this.Get<Teleport>();
    }

    private void OnEnable()
    {
        time.OnDayStart.AddListener(GoOutsideRoom);
    }

    private void OnDisable()
    {
        time.OnDayStart.RemoveListener(GoOutsideRoom);
    }

    public void SetResident(Town _resident)
    {
        tp.objToTeleport = _resident.gameObject;
        resident = _resident;
        AssignResidentToLocations();
    }

    void AssignResidentToLocations()
    {
        List<IResidentLocation> locations = gameObject.GetComponentsInChildren<MonoBehaviour>()
            .OfType<IResidentLocation>()
            .ToList();

        if(locations != null && locations.Count > 0) 
            locations.ForEach(l => l.resident = resident);

    }

    public void TelportBack() => tp.DoTeleport();

    public void GoOutsideRoom()
    {
        if (!resident) return;

        if (!resident.gameObject.Has(out NPC_Movement npc))
        {
            this.Log("EARLY RETURN: Did not find a room");
            return;
        }
        npc.area = outsideRoomArea;
        npc.agent.SetDestination(outsideRoomArea.GetARandLocation());
    }

    #region Methods
        
    #endregion

}
