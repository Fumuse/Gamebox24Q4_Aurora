using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
public class Condition
{
    public string Action { get => _action.GetLocalizedString();private set { } }
    [SerializeField] private LocalizedString _action;
    public bool Required;
}