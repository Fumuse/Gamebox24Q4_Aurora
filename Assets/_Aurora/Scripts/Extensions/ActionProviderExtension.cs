using JetBrains.Annotations;

public static class ActionProviderExtension
{
    public static void ChangeInteractableObjectAction(this IAction action, IInteractable interactable, 
        [CanBeNull] ActionSettings actionSettings, [CanBeNull] ActionEvent actionEvent)
    {
        if (actionEvent == null) return;
        interactable.ChangeActionProvider(actionEvent.GetEvent(actionSettings));
    }
}