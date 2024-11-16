using System.Threading;
using Cysharp.Threading.Tasks;

public class MoveTutorialState : TutorialBaseState
{
    private ActionSettings _actionSetting;
    private WhisperProvider _whisperProvider;
    private TeleportProvider _teleportProvider;

    private CancellationTokenSource _cts;

    private bool _grandmaCallsEnded = false;
    private bool _sayAboutMovingEnded = false;
    
    public MoveTutorialState(TutorialStateMachine stateMachine) : base(stateMachine)
    {}

    public override void Enter()
    {
        _cts = new();

        _teleportProvider = GameProvidersManager.Instance.TeleportProvider;
        _whisperProvider = GameProvidersManager.Instance.WhisperProvider;
        _whisperProvider.OnWhisperEnds += OnWhisperEnds;
        GrandmaCalls();
        SayAboutMoving();
    }

    private async void GrandmaCalls()
    {
        ActionSettings setting = GetSettingByKey("GrandmaCalls");
        if (setting == null) return;
        
        bool isCanceled = await UniTask.WaitForSeconds(.5f, cancellationToken: _cts.Token)
            .SuppressCancellationThrow();
        if (isCanceled) return;

        _whisperProvider.Execute(setting);
    }

    private async void SayAboutMoving()
    {
        bool isCanceled = await UniTask.WaitUntil(() => _grandmaCallsEnded, cancellationToken: _cts.Token)
            .SuppressCancellationThrow();
        if (isCanceled) return;

        ActionSettings setting = GetSettingByKey("TutorialMove");
        if (setting == null) return;
        
        //Включаем дверцу в Loc_2
        UnlockDoor("Room_1_DoorLeft");
        _teleportProvider.OnPlayerTeleported += OnPlayerTeleported;
        
        //Включаем движение игрока
        GameManager.Instance.InitPlayer();

        isCanceled = await UniTask.WaitForSeconds(.5f, cancellationToken: _cts.Token)
            .SuppressCancellationThrow();
        if (isCanceled) return;
        
        _whisperProvider.Execute(setting);
    }

    private void OnWhisperEnds()
    {
        if (!_grandmaCallsEnded)
        {
            _grandmaCallsEnded = true;
            return;
        }

        if (!_sayAboutMovingEnded)
        {
            _sayAboutMovingEnded = true;
        }
    }

    private void OnPlayerTeleported()
    {
        stateMachine.SwitchState(new TrellisWatchState(stateMachine));
    }

    public override void Tick()
    {}

    public override void Exit()
    {
        _whisperProvider.OnWhisperEnds -= OnWhisperEnds;
        _teleportProvider.OnPlayerTeleported -= OnPlayerTeleported;
    }
}