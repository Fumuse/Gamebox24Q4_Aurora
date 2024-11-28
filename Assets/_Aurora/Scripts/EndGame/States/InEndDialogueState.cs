using UnityEngine;

public class InEndDialogueState : State
{
    private EndGameStateMachine _stateMachine;
    private ShowVideoSceneProvider _videoProvider;
    private EndGamePath _path;
    private bool _happyEnd = true;

    public InEndDialogueState(EndGameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public override void Enter()
    {
        _videoProvider = GameProvidersManager.Instance.VideoSceneProvider;

        InteractableObject.OnCancelInteract += OnCancelInteract;
        WindowLoc6_DialogueProvider.EndDialoguePoints += EscapeEndDialoguePoints;
        PhotoAlbumLoc4_DialogueProvider.EndDialoguePoints += AcceptanceEndDialoguePoints;
    }

    public override void Tick()
    {
    }

    public override void Exit()
    {
        InteractableObject.OnCancelInteract -= OnCancelInteract;
        WindowLoc6_DialogueProvider.EndDialoguePoints -= EscapeEndDialoguePoints;
        PhotoAlbumLoc4_DialogueProvider.EndDialoguePoints -= AcceptanceEndDialoguePoints;
    }

    private void EscapeEndDialoguePoints(int points)
    {
        _path = EndGamePath.Escape;
        CheckHappyEnd(points);
    }

    private void AcceptanceEndDialoguePoints(int points)
    {
        _path = EndGamePath.Acceptance;
        CheckHappyEnd(points);
    }

    private void CheckHappyEnd(int points)
    {
        GameManager manager = GameManager.Instance;
        if (points >= 3 && manager.AcceptanceScale.Current <= manager.Settings.AcceptanceBadEndStep)
        {
            _happyEnd = false;
        }
    }

    private void OnCancelInteract(IInteractable interactable)
    {
        switch (_path)
        {
            case EndGamePath.Escape:
                _videoProvider.Execute(_happyEnd ? _stateMachine.WinEscapeSettings : _stateMachine.DeathMovieSettings);
                break;
            case EndGamePath.Acceptance:
                _videoProvider.Execute(_happyEnd ? _stateMachine.WinAcceptanceSettings : _stateMachine.DeathReplacementMovieSettings);
                break;
        }
    }

    private enum EndGamePath
    {
        Acceptance,
        Escape
    }
}