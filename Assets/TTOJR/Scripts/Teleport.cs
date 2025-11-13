using System.Collections;
using System.ComponentModel;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Extensions;

public class Teleport : MonoBehaviour
{
    bool teleporting;
    public Transform tpLoc;
    [Sirenix.OdinInspector.ReadOnly] public GameObject objToTeleport;
    Detector detector;

    private void Awake()
    {
        detector = GetComponent<Detector>();
        if (!tpLoc) this.Error($"Did not set a variable: " + "tpLoc: {tpLoc}");
    }
    public void DoTeleport()
    {
        if (teleporting) return;

        teleporting = true;
        print("Teleport: Attempting TP");
        if (objToTeleport == null) SetObjectToTeleportFromDetector();

        if (objToTeleport.Has(out TeleportFader tpFader))
            FadeTeleport(tpFader.fadeScreenRef);
        else
        {
            NavEX.Teleport(tpLoc, objToTeleport, out bool _teleporting);
            teleporting = _teleporting;
        }
    }

    public void FadeTeleport(FadeScreen fade)
    {
        fade.FadeInAndOutCallback(midhook: () => 
        {
            if (!tpLoc || !objToTeleport) { teleporting = false; return; }
            NavEX.Teleport(tpLoc, objToTeleport, out bool _teleporting);
            teleporting = _teleporting;
        });
    }

    public void SetObjectToTeleport(GameObject GO) => objToTeleport = GO;
    public void SetObjectToTeleportFromDetector()
    {
        if (detector.colliderObject == null)
            Debug.LogError("Teleport: Cannot assign obj to teleport, its null from detector");
        objToTeleport = detector.colliderObject;
    }
}
