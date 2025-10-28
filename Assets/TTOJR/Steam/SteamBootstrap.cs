using UnityEngine;
using Steamworks;

public class SteamBootstrap : MonoBehaviour
{
    static bool _inited;

    void Awake()
    {
        if (_inited) return;
        try
        {
            if (!SteamAPI.Init()) { Debug.LogError("SteamAPI.Init failed"); return; }
            _inited = true;
        }
        catch { Debug.LogError("Steam not found / not running"); }
    }

    void Update()
    {
        if (_inited) SteamAPI.RunCallbacks();
    }

    void OnDestroy()
    {
        if (_inited) { SteamAPI.Shutdown(); _inited = false; }
    }
}