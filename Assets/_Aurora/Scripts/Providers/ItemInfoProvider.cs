using System;
using System.Threading;
using UnityEngine;

public class ItemInfoProvider : MonoBehaviour, IAction
{
    private IInteractable _lastInteractable;
    private ActionSettings _actionSettings;
    private CancellationTokenSource _cts;

    [SerializeField] private ItemInfoHandler _itemInfoHandler;

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
        _actionSettings = settings;

        ItemInfo item = settings.Item;
        _itemInfoHandler.ShowItem(item);

        _lastInteractable.FinishInteract();
    }

    private void OnInteracted(IInteractable interactable)
    {
        _lastInteractable = interactable;
    }
}
