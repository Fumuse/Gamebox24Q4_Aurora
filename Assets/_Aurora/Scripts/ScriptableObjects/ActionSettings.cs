using UnityEngine;

[CreateAssetMenu(fileName = "ActionSettings", menuName = "Actions/ActionSettings")]
public class ActionSettings : ScriptableObject
{
    [SerializeField] private string whisperTextKey;
    public string WhisperTextKey => whisperTextKey;
}