using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "DialogueNode", menuName = "Dialogue/DialogueNode")]
public class DialogueNode : ScriptableObject
{
    public List<Dialogue> Dialogue;
    public string EndDialoguesID;
}

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
}

[System.Serializable]
public class Response
{
    public int ID;


    [SerializeField] private LocalizedString _tag;
    [SerializeField] private LocalizedString _responseText;
    [field: SerializeField] public DialogueNode NextDialogue { get; private set; }
    [field: SerializeField] public List<Condition> Condition { get;private set; }
    [field: SerializeField] public string ColorText { get; private set; }

    public LocalizedString Tag { get => _tag; private set { } }
    public LocalizedString ResponseText { get => _responseText; private set { } }
}

[System.Serializable]
public class Condition
{
    public string Action { get => _action.GetLocalizedString();private set { } }
    [SerializeField] private LocalizedString _action;
    public bool Required;
}

[System.Serializable]
public class DialogueEventObject
{
    [field: SerializeField] public string NameEvent { get; private set; }
    [field: SerializeField] public bool SendEvent { get; private set; }
    [field: SerializeField] public bool WaitEndEvent { get; private set; }
}