using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

[RequireComponent(typeof(IInteractable))]
public class InteractableObjectUI : MonoBehaviour
{
    [SerializeField] private Canvas objectMenu;
    [SerializeField] private Button interactButton;
    [SerializeField, HideInInspector] private InteractableObject interObject;
    [SerializeField, HideInInspector] private LocalizeStringEvent buttonTextLocalizeEvent;

    [Header("Objects sprites controller")] 
    [SerializeField] private SpriteRenderer bodySprite;
    [SerializeField] private List<InteractableObjectStateVisionPair> visionStates;

    public SpriteRenderer BodySprite => bodySprite;

    private bool _mouseIn = false;
    private GraphicRaycaster _menuRaycaster;

    private InteractableStateVisionEnum _currentStateVision = InteractableStateVisionEnum.Default;

    private void OnValidate()
    {
        objectMenu ??= GetComponentInChildren<Canvas>();
        interObject ??= GetComponent<InteractableObject>();
        interactButton ??= GetComponentInChildren<Button>();
        buttonTextLocalizeEvent ??= GetComponent<LocalizeStringEvent>();
    }

    private void Awake()
    {
        _menuRaycaster ??= objectMenu.GetComponent<GraphicRaycaster>();
    }

    private void OnEnable()
    {
        HideInteractMenu();
        interObject.onPreInteract += OnPreInteract;
        interObject.onDeclineInteract += OnDeclineInteract;
        InteractableObject.OnInteracted += OnInteracted;
        InteractableObject.OnCancelInteract += OnCancelInteract;
        
        interactButton.onClick.AddListener(interObject.Interact);
    }

    private void OnDisable()
    {
        interObject.onPreInteract -= OnPreInteract;
        interObject.onDeclineInteract -= OnDeclineInteract;
        InteractableObject.OnInteracted -= OnInteracted;
        InteractableObject.OnCancelInteract -= OnCancelInteract;
        
        interactButton.onClick.RemoveListener(interObject.Interact);
    }

    public void OnRayMouseEnter()
    {
        _mouseIn = true;
        if (interObject.IsInteracted) return;
        if (!interObject.IsViewed) return;
        if (interObject.IsInteractBlocked) return;
        ChangeBodySpriteByStage(GameManager.Instance.OppositeSpriteStage);
        ShowInteractMenu();
    }

    public void OnRayMouseExit()
    {
        _mouseIn = false;
        if (interObject.IsInteracted) return;
        ChangeBodySpriteByStage(GameManager.Instance.CurrentStage);
        HideInteractMenu();
    }

    public void ChangeBodySpriteByStage(HouseStageEnum stage)
    {
        if (visionStates == null) return;

        InteractableObjectStateVisionPair pair = visionStates.FirstOrDefault((statePair) => statePair.visionKey == _currentStateVision);
        if (pair == null)
            pair = visionStates.FirstOrDefault((statePair) => statePair.visionKey == InteractableStateVisionEnum.Default);
        if (pair == null) return;
        
        InteractableObjectState currentState = pair.interactableObjectState;
        if (currentState == null) return;
        
        if (stage == HouseStageEnum.Light && currentState.Light != null)
            bodySprite.sprite = currentState.Light;
        if (stage == HouseStageEnum.Dark && currentState.Dark != null)
            bodySprite.sprite = currentState.Dark;
        if (stage == HouseStageEnum.Broken && currentState.Broken != null)
            bodySprite.sprite = currentState.Broken;
    }
    
    private void ShowInteractMenu()
    {
        interactButton.interactable = interObject.IsInteracted;
        
        objectMenu.gameObject.SetActive(true);
    }

    private void HideInteractMenu()
    {
        _menuRaycaster.enabled = false;
        interactButton.interactable = interObject.IsInteracted;
        
        objectMenu.gameObject.SetActive(false);
    }

    private void OnPreInteract()
    {
        if (interObject.IsInteracted)
        {
            _menuRaycaster.enabled = true;
            ShowInteractMenu();
            ChangeBodySpriteByStage(GameManager.Instance.OppositeSpriteStage);
        }
    }

    private void OnDeclineInteract()
    {
        if (!interObject.IsInteracted && !_mouseIn)
        {
            HideInteractMenu();
            ChangeBodySpriteByStage(GameManager.Instance.CurrentStage);
        }
    }

    private void OnInteracted(IInteractable interactable)
    {
        if (!interObject.Equals(interactable)) return; 
        HideInteractMenu();
    }

    private void OnCancelInteract(IInteractable interactable)
    {
        if (!interObject.Equals(interactable)) return;
        OnDeclineInteract();
    }

    public void ChangeObjectName(string newNameLocalizationKey)
    {
        buttonTextLocalizeEvent.StringReference.TableEntryReference = newNameLocalizationKey;
    }

    public void ChangeVisionState(InteractableStateVisionEnum visionState)
    {
        _currentStateVision = visionState;
        ChangeBodySpriteByStage(GameManager.Instance.CurrentStage);
    }
}