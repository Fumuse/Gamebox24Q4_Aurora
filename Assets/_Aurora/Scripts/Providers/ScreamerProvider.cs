using System.Linq;
using System.Threading;
using UnityEngine;

public class ScreamerProvider : MonoBehaviour, IAction
{
    [SerializeField] private Screamer[] screamers;
    
    private IInteractable _lastInteractable;
    private ActionSettings _actionSettings;
    private CancellationTokenSource _cts = new();

    private void OnValidate()
    {
        if (screamers == null)
        {
            screamers = FindObjectsByType<Screamer>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToArray();
        }
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
        if (settings.ScreamerType != ScreamerEnum.Empty)
        {
            Screamer screamer =
                screamers.FirstOrDefault((screamElement) => screamElement.ScreamerType == settings.ScreamerType);
            if (screamer != null)
            {
                screamer.transform.localPosition = settings.ScreamerSpawnPosition;
                screamer.Activate(true);
            }
        }
        
        if (_lastInteractable != null)
        {
            if (_actionSettings != null)
            {
                this.AfterInteractChanges(_lastInteractable, _actionSettings);
            }
            _lastInteractable.FinishInteract();
        }
    }

    private void OnInteracted(IInteractable interactable)
    {
        _lastInteractable = interactable;
    }
}