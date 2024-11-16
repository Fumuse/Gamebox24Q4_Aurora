public interface IDialogue
{
    public void OnInteract();

    public void StartNewDialog(DialogueNode newDialogue);
}

public interface IDialogContinue
{
    void EndEvent();
}