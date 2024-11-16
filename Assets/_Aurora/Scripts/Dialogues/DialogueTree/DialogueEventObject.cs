using UnityEngine;

[System.Serializable]
public class DialogueEventObject
{
    [field: SerializeField] public string NameEvent { get; private set; }
    [field: SerializeField] public bool SendEvent { get; private set; }
    [field: SerializeField] public bool WaitEndEvent { get; private set; }
}