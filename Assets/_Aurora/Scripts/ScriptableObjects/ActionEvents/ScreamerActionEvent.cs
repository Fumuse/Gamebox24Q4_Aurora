using UnityEngine;

[CreateAssetMenu(fileName = "ScreamerActionEvent", menuName = "Actions/ActionEvents/ScreamerActionEvent")]
public class ScreamerActionEvent : ActionEvent
{
    private ScreamerProvider _screamerProvider;

    private void Init()
    {
        if (_screamerProvider == null)
        {
            _screamerProvider = GameProvidersManager.Instance.ScreamerProvider;
        }
    }

    public override ListedUnityEvent GetEvent(ActionSettings settings)
    {
        Init();
        
        ListedUnityEvent unityEvent = new ListedUnityEvent();
        unityEvent.AddListener(() => _screamerProvider.Execute(settings));
        return unityEvent;
    }
}