using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TeleportProvider : MonoBehaviour, IAction
{
    [SerializeField] private CanvasGroup overlayWrapper;
    [SerializeField] private Transform player;
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private float timeToWaitBetweenRooms = 1f;
    [SerializeField] protected Camera mainCamera;
    
    private CancellationTokenSource _cts;
    private IInteractable _lastInteractable;
    private Door _interactableDoor;
    private ActionSettings _actionSettings;

    private void OnValidate()
    {
        mainCamera ??= Camera.main;
    }

    private void OnEnable()
    {
        _cts = new();
        InteractableObject.OnInteracted += OnInteracted;
    }

    private void OnDisable()
    {
        _cts?.Cancel();
        InteractableObject.OnInteracted -= OnInteracted;
    }

    public void Execute(ActionSettings settings)
    {
        GetInteractableDoor();
        _actionSettings = settings;

        Teleport();
    }

    private void GetInteractableDoor()
    {
        if (!_lastInteractable.GameObject.TryGetComponent(out Door door))
        {
            _lastInteractable.FinishInteract();
            return;
        }

        _interactableDoor = door;
    }

    private async void Teleport()
    {
        bool isCanceled = await overlayWrapper.FadeIn(this, _cts.Token, fadeSpeed);
        if (isCanceled) return;

        TeleportPlayer();

        isCanceled = await UniTask.WaitForSeconds(timeToWaitBetweenRooms, cancellationToken: _cts.Token)
            .SuppressCancellationThrow();
        if (isCanceled) return;
        
        isCanceled = await overlayWrapper.FadeOut(this, _cts.Token, fadeSpeed);
        if (isCanceled) return;
        
        _interactableDoor.FinishInteract();
    }

    private void TeleportPlayer()
    {
        Vector3 newPlayerPosition = player.position;
        newPlayerPosition.x = _interactableDoor.ConnectedDoor.transform.position.x;

        player.position = newPlayerPosition;

        Vector3 connectedRoomPosition = _interactableDoor.ConnectedDoor.Room.transform.position;
        Vector3 newCameraPosition = mainCamera.transform.position;
        newCameraPosition.x = connectedRoomPosition.x;
        newCameraPosition.y = connectedRoomPosition.y;

        mainCamera.transform.position = newCameraPosition;
    }
    
    private void OnInteracted(IInteractable interactable)
    {
        _lastInteractable = interactable;
    }
}