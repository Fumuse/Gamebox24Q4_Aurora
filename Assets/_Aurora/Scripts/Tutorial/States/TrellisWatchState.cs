public class TrellisWatchState : TutorialBaseState
{
    private IInteractable _trellis;
    private TeleportProvider _teleportProvider;
    
    public TrellisWatchState(TutorialStateMachine stateMachine) : base(stateMachine)
    {}

    public override void Enter()
    {
        _trellis = GetInteractableByKey("Room_2_Trellis");
        if (_trellis == null) return;
        
        _teleportProvider = GameProvidersManager.Instance.TeleportProvider;
        
        InteractableObject.OnCancelInteract += OnCancelInteract;
    }

    public override void Tick()
    {}

    public override void Exit()
    {
        _teleportProvider.OnPlayerTeleported -= OnPlayerTeleported;
    }

    private void OnCancelInteract(IInteractable interactable)
    {
        if (!interactable.Equals(_trellis)) return;
        
        InteractableObject.OnCancelInteract -= OnCancelInteract;
        
        //Включаем дверцу в Loc_3
        UnlockDoor("Room_2_DoorLeft");
        _teleportProvider.OnPlayerTeleported += OnPlayerTeleported;
    }

    private void OnPlayerTeleported()
    {
        stateMachine.SwitchState(new AlbumWatchState(stateMachine));
    }
}