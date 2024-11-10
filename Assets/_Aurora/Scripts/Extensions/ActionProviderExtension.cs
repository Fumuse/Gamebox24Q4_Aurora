using JetBrains.Annotations;

public static class ActionProviderExtension
{
    public static void ChangeInteractableObjectAction(this IAction action, IInteractable interactable, 
        ActionSettings actionSettings, [CanBeNull] ActionEvent actionEvent)
    {
        if (actionEvent == null) return;
        interactable.ChangeActionProvider(actionEvent.GetEvent(actionSettings));
    }

    public static void AddingTagsAfterInteract(this IAction action, ActionSettings actionSettings)
    {
        if (actionSettings.TagsToAddAfterAction.Length < 1) return;

        foreach (Tag tag in actionSettings.TagsToAddAfterAction)
        {
            GameManager.Instance.TagManager.AddTag(tag);
        }
    }
}