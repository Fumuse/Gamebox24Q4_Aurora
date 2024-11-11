using System.Collections.Generic;
using UnityEngine.Localization;

public static class PlayerAction
{
    private static HashSet<string> _action = new HashSet<string>();

    public static void PerformAction(string action) =>_action.Add(action);

    public static bool HasAction(string action) => _action.Contains(action);

    public static void ClearAction()=>_action.Clear();
}
