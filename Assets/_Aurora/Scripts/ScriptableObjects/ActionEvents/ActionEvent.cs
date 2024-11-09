using JetBrains.Annotations;
using UnityEngine;

public abstract class ActionEvent : ScriptableObject
{
    public abstract ListedUnityEvent GetEvent([CanBeNull] ActionSettings settings);
}