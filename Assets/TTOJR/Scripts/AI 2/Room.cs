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

    public void SetResident(Town _resident)
    {
        tp.objToTeleport = _resident.gameObject;
        resident = _resident;
    }


    #region Methods
        
    #endregion

}
