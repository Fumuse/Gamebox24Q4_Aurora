using UnityEngine;

public class InEndDialogueState : State
{
    private EndGameStateMachine _stateMachine;
    private ShowVideoSceneProvider _videoProvider;
    private EndGamePath _path;
    
    public InEndDialogueState(EndGameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }
    
    public override void Enter()
    {
        Debug.Log(this);

        _videoProvider = GameProvidersManager.Instance.VideoSceneProvider;

        InteractableObject.OnCancelInteract += OnCancelInteract;
        WindowLoc6_DialogueProvider.EndDialoguePoints += EscapeEndDialoguePoints;
        PhotoAlbumLoc4_DialogueProvider.EndDialoguePoints += AcceptanceEndDialoguePoints;
    }

    public override void Tick() {}

    public override void Exit()
    {
        InteractableObject.OnCancelInteract -= OnCancelInteract;
        WindowLoc6_DialogueProvider.EndDialoguePoints -= EscapeEndDialoguePoints;
        PhotoAlbumLoc4_DialogueProvider.EndDialoguePoints -= AcceptanceEndDialoguePoints;
    }

    private void EscapeEndDialoguePoints(int points)
    {
        _path = EndGamePath.Escape;
    }

    private void AcceptanceEndDialoguePoints(int points)
    {
        _path = EndGamePath.Acceptance;
    }

    private void OnCancelInteract(IInteractable interactable)
    {
        if (_path == EndGamePath.Escape)
        {
            _videoProvider.Execute(_stateMachine.WinEscapeSettings);
        }
        else if (_path == EndGamePath.Acceptance)
        {
            _videoProvider.Execute(_stateMachine.WinAcceptanceSettings);
        }
    }
    
    private enum EndGamePath
    {
        Acceptance,
        Escape
    }
}