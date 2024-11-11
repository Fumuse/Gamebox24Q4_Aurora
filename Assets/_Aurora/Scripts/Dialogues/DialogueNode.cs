using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "DialogueNode", menuName = "Scriptable Objects/DialogueNode")]
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

    public string Tag { get => _tag.GetLocalizedString(); private set { } }
    public string DialogueText { get => _dialogueText.GetLocalizedString(); private set { } }

    public void SubscribeToEvents()
    {
        _tag.StringChanged += OnTagStringChange;
        _dialogueText.StringChanged += OnDialogueStrinChange;
    }

    public void UnsubscribeFromEvents()
    {
        _tag.StringChanged -= OnTagStringChange;
        _dialogueText.StringChanged -= OnDialogueStrinChange;
    }

    private void OnDialogueStrinChange(string dialogueText)=> DialogueText = dialogueText;

    private void OnTagStringChange(string tag)=>Tag = tag;
}

[System.Serializable]
public class Response
{
    public int ID;

    public string Tag { get => _tag.GetLocalizedString();private set { } }
    public string ResponseText { get => _responseText.GetLocalizedString(); private set { } }

    [SerializeField] private LocalizedString _tag;
    [SerializeField] private LocalizedString _responseText;
    [field: SerializeField] public DialogueNode NextDialogue { get; private set; }
    [field: SerializeField] public List<Condition> Condition { get;private set; }
    [field: SerializeField] public string ColorText { get; private set; }

    public void SubscribeToEvents()
    {
        _tag.StringChanged += OnTagStringChange;
        _responseText.StringChanged += OnResponseStrinChange;
    }

    public void UnsubscribeFromEvents()
    {
        _tag.StringChanged -= OnTagStringChange;
        _responseText.StringChanged -= OnResponseStrinChange;
    }

    private void OnResponseStrinChange(string responseText) => ResponseText = responseText;

    private void OnTagStringChange(string tag) => Tag = tag;

}

[System.Serializable]
public class Condition
{
    public string Action { get => _action.GetLocalizedString();private set { } }
    [SerializeField] private LocalizedString _action;
    public bool Required;

    public void SubscribeToEvents()
    {
        _action.StringChanged += OnActionStrinChange;
    }

    public void UnsubscribeFromEvents()
    {
        _action.StringChanged -= OnActionStrinChange;
    }

    private void OnActionStrinChange(string dialogueText) => Action = dialogueText;
}

[System.Serializable]
public class DialogueEventObject
{
    [field: SerializeField] public string NameEvent { get; private set; }
    [field: SerializeField] public bool SendEvent { get; private set; }
    [field: SerializeField] public bool WaitEndEvent { get; private set; }
}