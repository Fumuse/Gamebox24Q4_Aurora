using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "DialogueActionEvent", menuName = "Actions/ActionEvents/DialogueActionEvent")]
public class DialogueActionEvent : ActionEvent
{
    private DialogueProvider _dialogueProvider;

    private void Init()
    {
        if (_dialogueProvider == null)
        {
            _dialogueProvider = GameProvidersManager.Instance.DialogueProvider;
        }
    }

    public override ListedUnityEvent GetEvent(ActionSettings settings)
    {
        Init();
        
        ListedUnityEvent unityEvent = new ListedUnityEvent();
        unityEvent.AddListener(() => _dialogueProvider.Execute(settings));
        return unityEvent;
    }
}
