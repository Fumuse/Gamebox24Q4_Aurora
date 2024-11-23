using System.Threading;
using Cysharp.Threading.Tasks;

public class AlbumWatchState : TutorialBaseState
{
    private IInteractable _photoAlbum;
    private TeleportProvider _teleportProvider;
    private WhisperProvider _whisperProvider;

    private CancellationTokenSource _cts = new();
    
    public AlbumWatchState(TutorialStateMachine stateMachine) : base(stateMachine)
    {}

    public override void Enter()
    {
        _photoAlbum = GetInteractableByKey("Room_3_PhotoAlbum");
        if (_photoAlbum == null) return;
        
        _teleportProvider = GameProvidersManager.Instance.TeleportProvider;
        _whisperProvider = GameProvidersManager.Instance.WhisperProvider;

        InteractableObject.OnCancelInteract += OnCancelInteract;

        SayAboutPhotoAlbum();
    }

    public override void Tick()
    {}

    public override void Exit()
    {
        _teleportProvider.OnPlayerTeleported -= OnPlayerTeleported;
    }

    private async void SayAboutPhotoAlbum()
    {
        bool isCanceled = await UniTask.WaitForSeconds(.5f, cancellationToken: _cts.Token)
            .SuppressCancellationThrow();
        if (isCanceled) return;
        
        ActionSettings setting = GetSettingByKey("TutorialAskAboutPhotoAlbum");
        if (setting == null) return;
        _whisperProvider.Execute(setting);
    }
    
    private void OnCancelInteract(IInteractable interactable)
    {
        if (!interactable.Equals(_photoAlbum)) return;
        
        InteractableObject.BlockInteractedObject(_photoAlbum);
        InteractableObject.OnCancelInteract -= OnCancelInteract;
        
        //Включаем дверцу в Loc_4
        UnlockDoor("Room_3_DoorLeft");
        _teleportProvider.OnPlayerTeleported += OnPlayerTeleported;
    }

    private void OnPlayerTeleported()
    {
        InteractableObject.UnblockInteractedObject(_photoAlbum);
        stateMachine.SwitchState(new GrandmaKitchenState(stateMachine));
    }
}