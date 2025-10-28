using Steamworks;

public static class Achievements
{
    public static bool Unlock(string apiName)
    {
        if (!SteamAPI.IsSteamRunning()) return false;
        bool ok = SteamUserStats.SetAchievement(apiName);
        ok &= SteamUserStats.StoreStats();
        return ok;
    }

    public static bool Clear(string apiName)
    {
        if (!SteamAPI.IsSteamRunning()) return false;
        bool ok = SteamUserStats.ClearAchievement(apiName);
        ok &= SteamUserStats.StoreStats();
        return ok;
    }

    public static bool Progress(string apiName, uint cur, uint max)
    {
        if (!SteamAPI.IsSteamRunning()) return false;
        return SteamUserStats.IndicateAchievementProgress(apiName, cur, max);
    }
}