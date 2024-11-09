using System;
using System.Collections.Generic;
using UnityEngine.Events;

[Serializable]
public class ListedUnityEvent : UnityEvent
{
    private List<UnityAction> _actions = new();

    public int ActionsCount => _actions.Count + this.GetPersistentEventCount();
    
    public new void AddListener(UnityAction call)
    {
        _actions.Add(call);
        base.AddListener(call);
    }

    public new void RemoveListener(UnityAction call)
    {
        if (_actions.Contains(call)) _actions.Remove(call);
        base.RemoveListener(call);
    }
}