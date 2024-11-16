using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
public class Dialogue
{
    [SerializeField] private LocalizedString _tag;
    [SerializeField] private LocalizedString _dialogueText;
    [field: SerializeField] public List<Condition> Condition { get; private set; }
    [field: SerializeField] public Response[] Response { get; private set; }
    [field: SerializeField] public bool RepeatEndPhraze {get; private set; }
    [field: SerializeField] public DialogueEventObject Event { get; private set; }

    public LocalizedString Tag { get => _tag; private set { } }
    public LocalizedString DialogueText { get => _dialogueText; private set { } }

    [SerializeField] private bool showCloseDialogueButtonIfEnd = true;
    public bool ShowCloseDialogueButtonIfEnd => showCloseDialogueButtonIfEnd;
}