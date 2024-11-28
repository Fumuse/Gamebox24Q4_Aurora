using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

public class CellarState : TutorialBaseState
{
    private ActionSettings _actionSetting;
    private TeleportProvider _teleportProvider;
    private WhisperProvider _whisperProvider;
    private TagManager _tagManager;
    private Tag _interactCorpseTag;
    private IInteractable _grandmaCorpse;
    private IInteractable _candleBox;
    private IInteractable _lastInteractable;
    private PlayerStateMachine _player;

    private List<ActionSettings> _usedSettings = new();

    private CancellationTokenSource _cts;

    private bool _playerSawCorpseByFlashlight = false;
    private bool _playerHasCorpseTag = false;
    private bool _grandmaSays1Sentence = false;
    private bool _grandmaSays2Sentence = false;
    private bool _grandmaSaysFlashlightSentence = false;
    
    public CellarState(TutorialStateMachine stateMachine) : base(stateMachine)
    {}

    public override void Enter()
    {
        _cts = new();
        
        _teleportProvider = GameProvidersManager.Instance.TeleportProvider;
        _whisperProvider = GameProvidersManager.Instance.WhisperProvider;
        _tagManager = GameManager.Instance.TagManager;
        _player = GameManager.Instance.Player;

        _interactCorpseTag = new Tag(TagEnum.InteractWithGrandmaCorpse);
        
        _teleportProvider.OnPlayerTeleported += OnPlayerTeleportedToCellar;
        
        Flashlight.OnFlashlightFindObject += OnFlashlightFindObject;
        InteractableObject.OnInteracted += OnInteracted;
        InteractableObject.OnCancelInteract += OnCancelInteract;
        
        _grandmaCorpse = GetInteractableByKey("Room_5_GrandmaCorpse");
        _candleBox = GetInteractableByKey("Room_5_CandleBox");
    }

    private async void SayAboutInteract()
    {
        ActionSettings setting = GetSettingByKey("CellarInteract");
        if (setting == null) return;
        
        bool isCanceled = await UniTask.WaitForSeconds(1f, cancellationToken: _cts.Token)
            .SuppressCancellationThrow();
        if (isCanceled) return;
        
        _whisperProvider.EmptyExecute(setting);
        _usedSettings.Add(setting);
    }

    private async void GrandmaSaySentences()
    {
        _whisperProvider.OnWhisperEnds += OnWhisperEnds;

        GrandmaSaySentence("CellarGrandma_1Sentence");
        
        bool isCanceled = await UniTask.WaitUntil(() => _grandmaSays1Sentence, cancellationToken: _cts.Token)
            .SuppressCancellationThrow();
        if (isCanceled) return;

        GrandmaSaySentence("CellarGrandma_2Sentence");
        
        isCanceled = await UniTask.WaitUntil(() => _grandmaSays2Sentence, cancellationToken: _cts.Token)
            .SuppressCancellationThrow();
        if (isCanceled) return;

        if (_playerSawCorpseByFlashlight)
        {
            GrandmaSaySentence("CellarGrandma_FlashlightSentence");
            
            isCanceled = await UniTask.WaitUntil(() => _grandmaSaysFlashlightSentence, cancellationToken: _cts.Token)
                .SuppressCancellationThrow();
            if (isCanceled) return;
        }

        PreEscape();
    }

    private async void GrandmaSaySentence(string sentenceSettings)
    {
        ActionSettings setting = GetSettingByKey(sentenceSettings);
        if (setting == null) return;
        
        bool isCanceled = await UniTask.WaitForSeconds(.5f, cancellationToken: _cts.Token)
            .SuppressCancellationThrow();
        if (isCanceled) return;
        
        _whisperProvider.EmptyExecute(setting);
        _usedSettings.Add(setting);
    }

    private void PreEscape()
    {
        _player.UnblockMove();
        
        //Включаем проход в предбанник
        UnlockDoor("Room_5_DoorUp");
        _teleportProvider.OnTeleportEnds += OnPlayerTeleportedToFirstRoom;
    }

    public override void Tick()
    {
        if (!_playerHasCorpseTag)
        {
            _playerHasCorpseTag = _tagManager.HasTag(_interactCorpseTag);
            if (_playerHasCorpseTag)
            {
                GrandmaSaySentences();
            }
        }
    }

    public override void Exit()
    {
        _whisperProvider.OnWhisperEnds -= OnWhisperEnds;
        _teleportProvider.OnTeleportEnds -= OnPlayerTeleportedToFirstRoom;
        InteractableObject.OnInteracted -= OnInteracted;
        InteractableObject.OnCancelInteract -= OnCancelInteract;
    }

    private void OnPlayerTeleportedToCellar()
    {
        _teleportProvider.OnPlayerTeleported -= OnPlayerTeleportedToCellar;
        GameManager.Instance.CurrentStage = HouseStageEnum.Dark;

        SayAboutInteract();
    }

    private void OnPlayerTeleportedToFirstRoom(Room room)
    {
        InteractableObject.UnblockInteractedObject(_candleBox);
        stateMachine.SwitchState(new TryToEscapeStage(stateMachine));
    }

    private void OnWhisperEnds(ActionSettings actionSettings)
    {
        if (!_usedSettings.Contains(actionSettings)) return;
        
        if (!_grandmaSays1Sentence)
        {
            _grandmaSays1Sentence = true;
            return;
        }

        if (!_grandmaSays2Sentence)
        {
            _grandmaSays2Sentence = true;
            return;
        }

        if (!_grandmaSaysFlashlightSentence)
        {
            _grandmaSaysFlashlightSentence = true;
        }
    }

    private void OnFlashlightFindObject(IInteractable interactable)
    {
        if (!interactable.Equals(_grandmaCorpse)) return;

        _playerSawCorpseByFlashlight = true;

        Flashlight.OnFlashlightFindObject -= OnFlashlightFindObject;
    }

    private void OnInteracted(IInteractable interactable)
    {
        if (!interactable.Equals(_grandmaCorpse)) return;
        
        interactable.PuffAudio();
        
        _player.BlockMove();
        InteractableObject.BlockInteractedObject(_grandmaCorpse);
        _lastInteractable = interactable;
    }

    private void OnCancelInteract(IInteractable interactable)
    {
        if (!interactable.Equals(_candleBox)) return;

        if (_tagManager.HasTag(new Tag(TagEnum.CandleTaken)))
        {
            InteractableObject.BlockInteractedObject(_candleBox);
        }
    }
}