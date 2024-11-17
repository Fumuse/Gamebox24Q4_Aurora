using UnityEngine;

[CreateAssetMenu(fileName = "WhisperActionEvent", menuName = "Actions/ActionEvents/WhisperActionEvent")]
public class WhisperActionEvent : ActionEvent
{
    private WhisperProvider _whisperProvider;

    private void Init()
    {
        if (_whisperProvider == null)
        {
            _whisperProvider = GameProvidersManager.Instance.WhisperProvider;
        }
    }

    public override ListedUnityEvent GetEvent(ActionSettings settings)
    {
        Init();
        
        ListedUnityEvent unityEvent = new ListedUnityEvent();
        unityEvent.AddListener(() => _whisperProvider.Execute(settings));
        return unityEvent;
    }
}
