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
        if (!actionSettings.AddTagsInTutorial && GameManager.Instance.TutorialStage) return;

        foreach (Tag tag in actionSettings.TagsToAddAfterAction)
        {
            GameManager.Instance.TagManager.AddTag(tag);
        }
    }

    /// <summary>
    /// Spend time to interact
    /// </summary>
    public static void SpendTime(this IAction action, ActionSettings actionSettings)
    {
        GameManager.Instance.Timer.SpendTime(
            actionSettings.TimeCost
        );
    }

    public static void AfterInteractChanges(this IAction action, IInteractable interactable, ActionSettings actionSettings)
    {
        action.SpendTime(actionSettings);
        action.AddingTagsAfterInteract(actionSettings);
        action.ChangeInteractableObjectAction(
            interactable, 
            actionSettings.ChangeActionSettingsAfterPlay, 
            actionSettings.ChangeObjectEventAfterPlay
        );
    }
}