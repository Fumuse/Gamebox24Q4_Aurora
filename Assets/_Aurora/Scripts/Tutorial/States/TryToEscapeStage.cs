public class TryToEscapeStage : TutorialBaseState
{
    private IInteractable _exitDoor;
    private TeleportProvider _teleportProvider;
    
    public TryToEscapeStage(TutorialStateMachine stateMachine) : base(stateMachine)
    {}

    public override void Enter()
    {
        _exitDoor = GetInteractableByKey("Room_1_ExitHouseDoor");
        if (_exitDoor == null) return;
        
        LockDoor("Room_1_DoorDown");
        UnlockDoor("Room_1_ExitHouseDoor");
        _teleportProvider = GameProvidersManager.Instance.TeleportProvider;
        
        _teleportProvider.OnPlayerTeleported += OnPlayerTeleported;
        _teleportProvider.OnTeleportEnds += OnTeleportEnds;
        InteractableObject.OnCancelInteract += OnCancelInteract;
    }

    public override void Tick()
    {}

    public override void Exit()
    {
        _teleportProvider.OnPlayerTeleported -= OnPlayerTeleported;
        _teleportProvider.OnTeleportEnds -= OnTeleportEnds;
        InteractableObject.OnCancelInteract -= OnCancelInteract;
    }

    private void OnPlayerTeleported()
    {
        GameManager.Instance.TagManager.AddTag(new Tag(TagEnum.TutorialEnded));
    }

    private void OnTeleportEnds()
    {
        GameManager.Instance.EndTutorial();
    }

    private void OnCancelInteract(IInteractable interactable)
    {
        if (!interactable.Equals(_exitDoor)) return;
        UnlockDoor("Room_1_DoorLeft");
        LockDoor("Room_1_ExitHouseDoor");
    }
}