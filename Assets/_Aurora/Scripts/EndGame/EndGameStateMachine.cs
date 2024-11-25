using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EndGameStateMachine : StateMachine
{
    [SerializeField] private CanvasGroup overlayWrapper;
    [SerializeField] private SceneController controller;

    [Header("Настройки завершения игры")] 
    [SerializeField] private ActionSettingsKeyPair[] settings;
    [SerializeField] private List<DoorKeyPair> doorsMap;
    [SerializeField] private List<InteractableObjectsKeyPair> objectsMap;
    
    [Header("Катсцены на конец игры")]
    [SerializeField] private ActionSettings deathMovieSettings;
    [SerializeField] private ActionSettings deathReplacementMovieSettings;

    private CancellationTokenSource _cts = new();
    private ShowVideoSceneProvider _videoSceneProvider;

    public ActionSettingsKeyPair[] Settings => settings;
    public IReadOnlyList<InteractableObjectsKeyPair> ObjectsMap => objectsMap;
    public List<DoorKeyPair> DoorsMap => doorsMap;

    private void OnValidate()
    {
        if (doorsMap.Count < 1)
        {
            Door[] doors = FindObjectsByType<Door>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (Door door in doors)
            {
                DoorKeyPair pair = new DoorKeyPair
                {
                    door = door,
                    doorKey = door.name
                };

                doorsMap.Add(pair);
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        WindowLoc6_DialogueProvider.OnDeath += OnDeath;
        PhotoAlbumLoc4_DialogueProvider.OnDeath += OnPhotoAlbumDeath;
        Slenderman.PlayerDeadFromScreamer += OnPlayerDeadFromScreamer;
        Timer.OnTimeEnded += OnTimeEnded;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        WindowLoc6_DialogueProvider.OnDeath -= OnDeath;
        PhotoAlbumLoc4_DialogueProvider.OnDeath -= OnPhotoAlbumDeath;
        Slenderman.PlayerDeadFromScreamer -= OnPlayerDeadFromScreamer;
        ShowVideoSceneProvider.OnVideoHiddenAfterEnd -= OnVideoHiddenAfterEnd;
        Timer.OnTimeEnded -= OnTimeEnded;
    }

    private void OnDeath()
    {
        _videoSceneProvider = GameProvidersManager.Instance.VideoSceneProvider;
        _videoSceneProvider.Execute(deathMovieSettings);

        ShowVideoSceneProvider.OnVideoHiddenAfterEnd += OnVideoHiddenAfterEnd;
    }

    private void OnPhotoAlbumDeath()
    {        
        _videoSceneProvider = GameProvidersManager.Instance.VideoSceneProvider;
        _videoSceneProvider.Execute(deathReplacementMovieSettings);

        ShowVideoSceneProvider.OnVideoHiddenAfterEnd += OnVideoHiddenAfterEnd;
    }

    private async void OnPlayerDeadFromScreamer(PlayerStateMachine player)
    {
        player.Die();
        
        bool isCanceled = await overlayWrapper.FadeIn(this, _cts.Token, 1f);
        if (isCanceled) return;

        OnDeath();
    }

    private void OnVideoHiddenAfterEnd()
    {
        // (bool isCanceled, Vector2 mousePosition) = await WaitForMouseClick()
        //     .AttachExternalCancellation(_cts.Token).SuppressCancellationThrow();
        // if (isCanceled) return;
        
        controller.GameEnd();
    }

    private async UniTask<Vector2> WaitForMouseClick()
    {
        UniTaskCompletionSource<Vector2> tcs = new UniTaskCompletionSource<Vector2>();
        InputReader.MouseClicked handler = (mousePosition) => tcs.TrySetResult(mousePosition);
        InputReader.OnMouseClicked += handler;
        
        return await tcs.Task;
    }

    private async void OnTimeEnded()
    {
        Timer.OnTimeEnded -= OnTimeEnded;

        PlayerStateMachine player = GameManager.Instance.Player;
        bool isCanceled = await UniTask.WaitWhile(() => player.InInteract, cancellationToken: _cts.Token)
            .SuppressCancellationThrow();
        if (isCanceled) return;
        
        SwitchState(new TimeEndedState(this));
    }
    
    private void LockDoor(string doorKey)
    {
        foreach (DoorKeyPair pair in DoorsMap)
        {
            if (pair.doorKey != doorKey) continue;
            pair.door.gameObject.SetActive(false);
        }
    }
    
    public IInteractable GetInteractableByKey(string key)
    {
        InteractableObjectsKeyPair setting = ObjectsMap.FirstOrDefault(
            (setting) => setting.key == key
        );
        
        if (setting == null)
        {
            throw new Exception($"{key} interactable object not founded.");
        }

        return setting.interactable;
    }

    public Door GetDoorByKey(string doorKey)
    {
        foreach (DoorKeyPair pair in DoorsMap)
        {
            if (pair.doorKey != doorKey) continue;
            return pair.door;
        }

        return null;
    }

    public void LockAllDoors()
    {
        foreach (DoorKeyPair pair in doorsMap)
        {
            pair.door.gameObject.SetActive(false);
        }
    }
}