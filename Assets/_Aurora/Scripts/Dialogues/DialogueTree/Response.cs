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
    [field: SerializeField] public ObjectCondition Condition { get; private set; }
    
    [SerializeField] private Tag[] tagsToAddAfterAction;
    public Tag[] TagsToAddAfterAction => tagsToAddAfterAction;

    public LocalizedString Tag
    {
        get => _tag;
        private set { }
    }

    public LocalizedString ResponseText
    {
        get => _responseText;
        private set { }
    }
}