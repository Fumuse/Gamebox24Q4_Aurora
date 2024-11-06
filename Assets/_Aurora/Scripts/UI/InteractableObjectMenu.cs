using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(IInteractable))]
public class InteractableObjectMenu : MonoBehaviour
{
    [SerializeField] private Canvas objectMenu;
    [SerializeField, HideInInspector] private InteractableObject interObject;
    [SerializeField] private Button interactButton;

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

    private void OnMouseEnter()
    {
        _mouseIn = true;
        if (interObject.IsInteracted) return;
        ShowInteractMenu();
    }

    private void OnMouseExit()
    {
        _mouseIn = false;
        if (interObject.IsInteracted) return;
        HideInteractMenu();
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
        }
    }

    private void OnDeclineInteract()
    {
        if (!interObject.IsInteracted && !_mouseIn)
        {
            HideInteractMenu();
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