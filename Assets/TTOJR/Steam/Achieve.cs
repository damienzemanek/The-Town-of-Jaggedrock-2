using Sirenix.OdinInspector;
using Steamworks;
using UnityEngine;

public class Achieve : MonoBehaviour
{

    #region Privates

    #endregion

    public void Acheivement()
    {
        Achievements.Unlock("ACH_WIN_ONE_GAME");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            Debug.Log("Unlock attempt");
            Achievements.Unlock("ACH_WIN_ONE_GAME");
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            Debug.Log("Resetting all Steam achievements for testing...");
            SteamUserStats.ResetAllStats(true);
            SteamUserStats.StoreStats();
        }

        // Press F11 to print all achievements to the Console
        if (Input.GetKeyDown(KeyCode.End))
        {
            if (!SteamAPI.IsSteamRunning())
            {
                Debug.LogWarning("Steam not running.");
                return;
            }

            uint count = SteamUserStats.GetNumAchievements();
            Debug.Log($"Total achievements: {count}");

            for (uint i = 0; i < count; i++)
            {
                string name = SteamUserStats.GetAchievementName(i);
                bool achieved;
                SteamUserStats.GetAchievement(name, out achieved);

                string desc = SteamUserStats.GetAchievementDisplayAttribute(name, "desc");
                string displayName = SteamUserStats.GetAchievementDisplayAttribute(name, "name");
                Debug.Log($"{i + 1}. {displayName} ({name}) - {(achieved ? "✅ Unlocked" : "❌ Locked")} — {desc}");
            }
        }
    }

//    [Button]
//#if UNITY_EDITOR
//    private void OnApplicationQuit()
//    {
//        Debug.Log("Resetting all Steam achievements for testing...");
//        SteamUserStats.ResetAllStats(true);
//        SteamUserStats.StoreStats();
//    }
//#endif


}
