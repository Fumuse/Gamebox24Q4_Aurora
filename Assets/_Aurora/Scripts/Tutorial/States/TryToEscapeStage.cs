using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

public class TryToEscapeStage : TutorialBaseState
{
    private IInteractable _exitDoor;
    private TeleportProvider _teleportProvider;
    private WhisperProvider _whisperProvider;

    private CancellationTokenSource _cts = new();

    private bool _auroraSaysEnd = false;

    private List<ActionSettings> _usedSettings = new();
    
    public TryToEscapeStage(TutorialStateMachine stateMachine) : base(stateMachine)
    {}

    public override void Enter()
    {
        _exitDoor = GetInteractableByKey("Room_1_ExitHouseDoor");
        if (_exitDoor == null) return;
        
        LockDoor("Room_1_DoorDown");
        UnlockDoor("Room_1_ExitHouseDoor");
        _teleportProvider = GameProvidersManager.Instance.TeleportProvider;
        
        _whisperProvider = GameProvidersManager.Instance.WhisperProvider;
        _whisperProvider.OnWhisperEnds += OnWhisperEnds;
        
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

    private void OnTeleportEnds(Room room)
    {
        AuroraSays();
        SayAboutExploring();
        GameManager.Instance.EndTutorial();
    }

    private async void AuroraSays()
    {
        ActionSettings setting = GetSettingByKey("TutorialEnd_AuroraSays");
        if (setting == null) return;        
        
        bool isCanceled = await UniTask.WaitForSeconds(.5f, cancellationToken: _cts.Token)
            .SuppressCancellationThrow();
        if (isCanceled) return;

        _whisperProvider.EmptyExecute(setting);
        _usedSettings.Add(setting);
    }

    private async void SayAboutExploring()
    {
        bool isCanceled = await UniTask.WaitUntil(() => _auroraSaysEnd, cancellationToken: _cts.Token)
            .SuppressCancellationThrow();
        if (isCanceled) return;
        
        ActionSettings setting = GetSettingByKey("TutorialEnd_SayAboutExploring");
        if (setting == null) return;        
        
        isCanceled = await UniTask.WaitForSeconds(.5f, cancellationToken: _cts.Token)
            .SuppressCancellationThrow();
        if (isCanceled) return;

        _whisperProvider.EmptyExecute(setting);
        _usedSettings.Add(setting);
    }

    private void OnWhisperEnds(ActionSettings actionSettings)
    {
        if (!_auroraSaysEnd)
        {
            _auroraSaysEnd = true;
        }
    }

    private void OnCancelInteract(IInteractable interactable)
    {
        if (!interactable.Equals(_exitDoor)) return;
        UnlockDoor("Room_1_DoorLeft");
        LockDoor("Room_1_ExitHouseDoor");
    }
}