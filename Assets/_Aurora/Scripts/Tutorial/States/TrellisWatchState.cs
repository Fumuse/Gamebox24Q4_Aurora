using System.Threading;

public class TrellisWatchState : TutorialBaseState
{
    private IInteractable _trellis;
    private TeleportProvider _teleportProvider;
    private WhisperProvider _whisperProvider;

    private InteractableObject _trellisObject;

    private CancellationTokenSource _cts = new();
    
    public TrellisWatchState(TutorialStateMachine stateMachine) : base(stateMachine)
    {}

    public override void Enter()
    {
        _trellis = GetInteractableByKey("Room_2_Trellis");
        if (_trellis == null) return;
        
        _trellisObject = _trellis.GameObject.GetComponent<InteractableObject>();
        _trellisObject.onPreInteract += OnPreInteractTrellis;
        
        _teleportProvider = GameProvidersManager.Instance.TeleportProvider;
        _whisperProvider = GameProvidersManager.Instance.WhisperProvider;
        
        InteractableObject.OnCancelInteract += OnCancelInteract;

        SayAboutInteract();
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
        
        InteractableObject.BlockInteractedObject(_trellis);
        InteractableObject.OnCancelInteract -= OnCancelInteract;
        
        //Включаем дверцу в Loc_3
        UnlockDoor("Room_2_DoorLeft");
        _teleportProvider.OnPlayerTeleported += OnPlayerTeleported;
    }

    private void OnPlayerTeleported()
    {
        InteractableObject.UnblockInteractedObject(_trellis);
        stateMachine.SwitchState(new AlbumWatchState(stateMachine));
    }

    private void SayAboutInteract()
    {
        ActionSettings setting = GetSettingByKey("TutorialInteract_1");
        if (setting == null) return;
        _whisperProvider.Execute(setting);
    }

    private void SayAboutInteractMenu()
    {
        ActionSettings setting = GetSettingByKey("TutorialInteract_2");
        if (setting == null) return;
        _whisperProvider.Execute(setting);
    }

    private void OnPreInteractTrellis()
    {
        _trellisObject.onPreInteract -= OnPreInteractTrellis;
        
        _cts?.Cancel();
        _cts = new();

        SayAboutInteractMenu();
    }
}