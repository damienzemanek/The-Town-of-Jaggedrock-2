using UnityEngine;

namespace Extensions
{
    public static class DebugExtensions
    {
        const string NullColor = "#AAAAAA";
        const string ScriptColor = "#00FF00";   // Green
        const string GameObjectColor = "#FFA500"; // Orange

        static string Colorize(string text, string colorHex) => $"<color={colorHex}>{text}</color>";
        static string Bold(string text) => $"<b>{text}</b>";



        public static void Log(this object obj, string msg = "")
        {
            if (obj == null)
            {
                Debug.Log($"{Colorize("[<null>]", NullColor)}: {msg}");
                return;
            }

            if (obj is Object unityObj)
                Debug.Log($"{Colorize($"[SCRIPT: {Bold(unityObj.GetType().Name)}]", ScriptColor)} " +
                          $"{Colorize($"[G.O.: {Bold(unityObj.name)}]", GameObjectColor)}: {msg}");
            else
                Debug.Log($"{Colorize($"[SCRIPT: {Bold(obj.GetType().Name)}]", ScriptColor)}: {msg}");
        }

        public static void Warn(this object obj, string msg = "")
        {
            if (obj == null)
            {
                Debug.LogWarning($"{Colorize("[<null>]", NullColor)}: {msg}");
                return;
            }

            if (obj is Object unityObj)
                Debug.LogWarning($"{Colorize($"[SCRIPT: {Bold(unityObj.GetType().Name)}]", ScriptColor)} " +
                                 $"{Colorize($"[G.O.: {Bold(unityObj.name)}]", GameObjectColor)}: {msg}");
            else
                Debug.LogWarning($"{Colorize($"[SCRIPT: {Bold(obj.GetType().Name)}]", ScriptColor)}: {msg}");
        }
        public static void Error(this object obj, string msg = "")
        {
            if (obj == null)
            {
                Debug.LogError($"{Colorize("[<null>]", NullColor)}: {msg}");
                return;
            }

            if (obj is Object unityObj)
                Debug.LogError($"{Colorize($"[SCRIPT: {Bold(unityObj.GetType().Name)}]", ScriptColor)} " +
                               $"{Colorize($"[G.O.: {Bold(unityObj.name)}]", GameObjectColor)}: {msg}");
            else
                Debug.LogError($"{Colorize($"[SCRIPT: {Bold(obj.GetType().Name)}]", ScriptColor)}: {msg}");
        }

        public static void SetCheck(this object obj)
        {
            if (obj == null)
                throw new System.ArgumentNullException($"{Colorize("[<null>]", NullColor)}: not set in inspector — please set");

            if (obj is Object unityObj)
                Debug.LogError($"{Colorize($"[SCRIPT: {Bold(unityObj.GetType().Name)}]", ScriptColor)} " +
                               $"{Colorize($"[G.O.: {Bold(unityObj.name)}]", GameObjectColor)}: field not assigned in inspector");
            else
                Debug.LogError($"{Colorize($"[SCRIPT: {Bold(obj.GetType().Name)}]", ScriptColor)}: field not assigned in inspector");
        }
    }
}