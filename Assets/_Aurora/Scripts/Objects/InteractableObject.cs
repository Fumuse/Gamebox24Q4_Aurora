using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(InteractableObjectMenu))]
public class InteractableObject : MonoBehaviour, IInteractable
{
    [SerializeField] protected float positionOffset;
    [SerializeField] protected UnityEvent actionProvider;
    [SerializeField] protected InteractableObjectCondition conditionToView;

    public bool IsInteracted { get; private set; }
    public Vector3 Position => this.transform.position;
    public float Offset => positionOffset;

    public GameObject GameObject => this.gameObject;

    #region События
    public Action onPreInteract;
    public Action onDeclineInteract;
    #endregion
    
    #region События делегаты
    public delegate void Interacted(IInteractable interactable);
    public static event Interacted OnInteracted;
    public delegate void CancelInteract(IInteractable interactable);
    public static event CancelInteract OnCancelInteract;
    #endregion

    protected void OnEnable()
    {
        CheckConditionToView();
    }

    //TODO: эта проверка должна быть у каждого объекта после всех триггеров
    /// <summary>
    /// Проверка условий отображения объекта
    /// </summary>
    public void CheckConditionToView()
    {
        if (conditionToView == null)
        {
            this.gameObject.SetActive(true);
            return;
        }

        bool needToHide = false;
        needToHide |= conditionToView.PassesTagsCondition;
        needToHide |= conditionToView.PassesTimeCondition;
        needToHide |= conditionToView.PassesAcceptanceCondition;

        this.gameObject.SetActive(!needToHide);
    }

    /// <summary>
    /// Взаимодействие с интерактивным объектом
    /// </summary>
    public virtual void Interact()
    {
        OnInteracted?.Invoke(this);
        
        actionProvider?.Invoke();
        if (actionProvider?.GetPersistentEventCount() < 1)
        {
            FinishInteract();
        }
    }

    /// <summary>
    /// После нажатия на интерактивный объект, но перед непосредственным взаимодействием
    /// </summary>
    public void PreInteract()
    {
        IsInteracted = true;
        onPreInteract?.Invoke();
    }

    /// <summary>
    /// Отмена взаимодействия с объектом
    /// </summary>
    public void DeclineInteract()
    {
        IsInteracted = false;
        onDeclineInteract?.Invoke();
    }

    /// <summary>
    /// Завершение взаимодействия с объектом
    /// </summary>
    public void FinishInteract()
    {
        IsInteracted = false;
        OnCancelInteract?.Invoke(this);
    }
}