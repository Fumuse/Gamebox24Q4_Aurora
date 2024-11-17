using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
public class Response
{
    [SerializeField] private DialogueEventsTypeEnum eventType = DialogueEventsTypeEnum.Empty;
    public DialogueEventsTypeEnum EventType => eventType;
    
    [SerializeField] public LocalizedSprite imageToShowAfterResponse;

    [SerializeField] private LocalizedString _tag;
    [SerializeField] private LocalizedString _responseText;
    [field: SerializeField] public DialogueNode NextDialogue { get; private set; }
    [field: SerializeField] public List<Condition> Condition { get;private set; }
    [field: SerializeField] public string ColorText { get; private set; }

    public LocalizedString Tag { get => _tag; private set { } }
    public LocalizedString ResponseText { get => _responseText; private set { } }
}