using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Extensions;

public class Teleport : MonoBehaviour
{
    public Transform tpLoc;
    [Sirenix.OdinInspector.ReadOnly] public NavEX.Teleportable tpable;
    Detector detector;

    private void Awake()
    {
        detector = GetComponent<Detector>();
        if (!tpLoc) this.Error($"Did not set a variable: " + "tpLoc: {tpLoc}");
    }
    public void DoTeleport()
    {
        this.Log("Teleport: Attempting TP");

        if(tpable.isTeleporting)
        { this.Log($"EARLY RETURN: tpable {tpable.objToTeleport.name} is already teleporting"); return; }

        if (tpable.objToTeleport == null) SetObjectToTeleportFromDetector();

        if (tpable.objToTeleport.Has(out TeleportFader tpFader))
            FadeTeleport(tpFader.fadeScreenRef);
        else
            NavEX.Teleport(tpLoc, ref tpable);


        this.Log($"Succesfully teleported obj {tpable.objToTeleport.name}");
    }

    public void FadeTeleport(FadeScreen fade)
    {
        fade.FadeInAndOutCallback(midhook: () => 
        {
            if (!tpLoc || !tpable.objToTeleport) { tpable.isTeleporting = false; return; }
            NavEX.Teleport(tpLoc, ref tpable);
        });
    }

    public void SetObjectToTeleport(GameObject GO) => tpable.objToTeleport = GO;
    public void SetObjectToTeleportFromDetector()
    {
        if (detector.colliderObject == null)
            Debug.LogError("Teleport: Cannot assign obj to teleport, its null from detector");
        tpable.objToTeleport = detector.colliderObject;
    }
}
