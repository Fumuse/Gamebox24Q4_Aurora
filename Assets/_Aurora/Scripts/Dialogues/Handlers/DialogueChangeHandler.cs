using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class DialogueChangeHandler : MonoBehaviour
{
    [SerializeField] private CanvasGroup overlayWrapper;
    
    [SerializeField] private InteractableObjectsKeyPair[] interactableObjectsToChange;
    [SerializeField] private DialogueView dialogueView;
    [SerializeField] private SpriteLibraryAsset spriteAssets;
    [SerializeField] private DialogueNodeLibraryAsset dialoguesAssets;

    private InteractableObject[] _interObjects;
    private Dictionary<string, IChangeDialogueProvider> _providers = new();

    private IInteractable _lastInteractable;
    private ActionSettings _lastActionSettings;

    private void Start()
    {
        _interObjects = interactableObjectsToChange.Select(pair => pair.interactable).ToArray();
    }

    private void OnEnable()
    {
        DialogueProvider.OnDialogEnded += OnDialogEnded;
        DialogueProvider.OnDialogueChangeHandler += OnDialogueChangeHandler;
        DialogueHandler.OnDialogueChangAfterResponseHandler += OnDialogueChangAfterResponseHandler;
    }

    private void OnDisable()
    {
        DialogueProvider.OnDialogEnded -= OnDialogEnded;
        DialogueProvider.OnDialogueChangeHandler -= OnDialogueChangeHandler;
        DialogueHandler.OnDialogueChangAfterResponseHandler -= OnDialogueChangAfterResponseHandler;
    }

    private void OnDialogueChangeHandler(IInteractable interactable, ref DialogueNode dialogueNode, ActionSettings actionSettings)
    {
        _lastInteractable = interactable;
        _lastActionSettings = actionSettings;
        
        if (!_interObjects.Contains(interactable)) return;

        InteractableObjectsKeyPair pair =
            interactableObjectsToChange.FirstOrDefault((pair) => pair.interactable.Equals(interactable));
        if (pair == null) return;

        IChangeDialogueProvider provider = null;
        if (_providers.ContainsKey(pair.key))
        {
            _providers.TryGetValue(pair.key, out provider);
        }
        else
        {
            switch (pair.key)
            {
                case "Loc_6_Window":
                {
                    provider = new WindowLoc6_DialogueProvider(dialogueView, spriteAssets, dialoguesAssets);
                    _providers.Add(pair.key, provider);
                    break;
                }
                case "Loc_4_PhotoAlbum":
                {
                    provider = new PhotoAlbumLoc4_DialogueProvider(dialogueView, spriteAssets, dialoguesAssets);
                    _providers.Add(pair.key, provider);
                    break;
                }
            }
        }

        if (provider != null)
        {
            overlayWrapper.alpha = 1;
            provider.ChangeDialogue(ref dialogueNode, actionSettings);
        }
    }

    private void OnDialogueChangAfterResponseHandler(ref DialogueNode dialogueNode)
    {
        OnDialogueChangeHandler(_lastInteractable, ref dialogueNode, _lastActionSettings);
    }

    private void OnDialogEnded()
    {
        _providers.Clear();
    }
}