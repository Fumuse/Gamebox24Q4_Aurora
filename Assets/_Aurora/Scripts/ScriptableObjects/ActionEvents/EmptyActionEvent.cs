using UnityEngine;

[CreateAssetMenu(fileName = "EmptyActionEvent", menuName = "Actions/ActionEvents/EmptyActionEvent")]
public class EmptyActionEvent : ActionEvent
{
    public override ListedUnityEvent GetEvent(ActionSettings settings)
    {
        ListedUnityEvent unityEvent = new ListedUnityEvent();
        return unityEvent;
    }
}