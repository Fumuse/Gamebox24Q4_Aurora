using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(IInteractable))]
public class InteractableObjectUI : MonoBehaviour
{
    [SerializeField] private Canvas objectMenu;
    [SerializeField, HideInInspector] private InteractableObject interObject;
    [SerializeField] private Button interactButton;

    [Header("Objects sprites controller")] 
    [SerializeField] private SpriteRenderer bodySprite;
    [SerializeField] private InteractableObjectState states;

    private bool _mouseIn = false;
    private GraphicRaycaster _menuRaycaster;

    private void OnValidate()
    {
        objectMenu ??= GetComponentInChildren<Canvas>();
        interObject ??= GetComponent<InteractableObject>();
        interactButton ??= GetComponentInChildren<Button>();
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
    
    //TODO: изображения должны переключаться в зависимости от общего стейта игры, продумать
    private void OnMouseEnter()
    {
        _mouseIn = true;
        if (interObject.IsInteracted) return;
        ChangeBodySpriteByStage(GameManager.Instance.OppositeSpriteStage);
        ShowInteractMenu();
    }

    private void OnMouseExit()
    {
        _mouseIn = false;
        if (interObject.IsInteracted) return;
        ChangeBodySpriteByStage(GameManager.Instance.CurrentStage);
        HideInteractMenu();
    }

    public void ChangeBodySpriteByStage(HouseStageEnum stage)
    {
        if (states == null) return;
        
        if (stage == HouseStageEnum.Light && states.Light != null)
            bodySprite.sprite = states.Light;
        if (stage == HouseStageEnum.Dark && states.Dark != null)
            bodySprite.sprite = states.Dark;
        if (stage == HouseStageEnum.Broken && states.Broken != null)
            bodySprite.sprite = states.Broken;
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
}