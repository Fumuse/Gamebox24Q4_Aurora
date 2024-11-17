public class TryToEscapeStage : TutorialBaseState
{
    private TeleportProvider _teleportProvider;
    
    public TryToEscapeStage(TutorialStateMachine stateMachine) : base(stateMachine)
    {}

    public override void Enter()
    {
        _teleportProvider = GameProvidersManager.Instance.TeleportProvider;
        
        _teleportProvider.OnPlayerTeleported += OnPlayerTeleported;
    }

    public override void Tick()
    {}

    public override void Exit()
    {
        _teleportProvider.OnPlayerTeleported -= OnPlayerTeleported;
    }

    private void OnPlayerTeleported()
    {
        GameManager.Instance.EndTutorial();
    }
}