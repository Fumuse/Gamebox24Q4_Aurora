using System.Linq;
using UnityEngine;

public class StartMovieState : TutorialBaseState
{
    private ShowVideoSceneProvider _provider;
    private ActionSettings _actionSetting;
    
    public StartMovieState(TutorialStateMachine stateMachine) : base(stateMachine)
    {}

    public override void Enter()
    {
        _provider = GameProvidersManager.Instance.VideoSceneProvider;
        stateMachine.OverlayWrapper.alpha = 1;

        ActionSettingsKeyPair setting = stateMachine.SettingsMap.FirstOrDefault(
                (setting) => setting.key == "TutorialMovie"
            );
        
        if (setting == null)
        {
            Debug.LogError("TutorialMovie settings not founded.");
            return;
        }
        
        _actionSetting = setting.actionSetting;

        _provider.OnVideoEndShowed += OnVideoEndShowed;
        _provider.Execute(_actionSetting);
    }

    public override void Tick()
    {}

    public override void Exit()
    {
        _provider.OnVideoEndShowed -= OnVideoEndShowed;
    }

    private void OnVideoEndShowed()
    {
        stateMachine.OverlayWrapper.alpha = 0;
        stateMachine.SwitchState(new MoveTutorialState(stateMachine));
    }
}