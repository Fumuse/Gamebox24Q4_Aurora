using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Tables;
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
    [SerializeField] private InteractableObjectState states;

    private bool _mouseIn = false;
    private GraphicRaycaster _menuRaycaster;

    private void OnValidate()
    {
        objectMenu ??= GetComponentInChildren<Canvas>();
        interObject ??= GetComponent<InteractableObject>();
        interactButton ??= GetComponentInChildren<Button>();
        buttonTextLocalizeEvent ??= interactButton.GetComponentInChildren<LocalizeStringEvent>();
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

    public void ChangeObjectName(string newNameLocalizationKey)
    {
        buttonTextLocalizeEvent.StringReference.TableEntryReference = newNameLocalizationKey;
    }
}