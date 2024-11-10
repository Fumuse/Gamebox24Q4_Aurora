public class AlbumWatchState : TutorialBaseState
{
    private IInteractable _photoAlbum;
    private TeleportProvider _teleportProvider;
    
    public AlbumWatchState(TutorialStateMachine stateMachine) : base(stateMachine)
    {}

    public override void Enter()
    {
        _photoAlbum = GetInteractableByKey("Room_3_PhotoAlbum");
        if (_photoAlbum == null) return;
        
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
        if (!interactable.Equals(_photoAlbum)) return;
        
        InteractableObject.OnCancelInteract -= OnCancelInteract;
        
        //Включаем дверцу в Loc_4
        UnlockDoor("Room_3_DoorLeft");
        _teleportProvider.OnPlayerTeleported += OnPlayerTeleported;
    }

    private void OnPlayerTeleported()
    {
        stateMachine.SwitchState(new GrandmaKitchenState(stateMachine));
    }
}