using System;
using UnityEngine;

[RequireComponent(typeof(InteractableObjectUI))]
public class InteractableObject : MonoBehaviour, IInteractable, IIlluminated
{
    [SerializeField] protected float positionOffset;
    [SerializeField] protected Transform objectPosition;
    [SerializeField] protected ListedUnityEvent actionProvider;
    [SerializeField] protected InteractableObjectCondition conditionToView;
    [SerializeField] private InteractableObjectUI ui;

    public bool IsInteracted { get; private set; }
    public Vector3 Position => objectPosition.position;
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

    protected bool _isViewed = false;
    public bool IsViewed => _isViewed;

    private void OnValidate()
    {
        objectPosition ??= this.transform;
        ui ??= GetComponent<InteractableObjectUI>();
    }

    public virtual void Init()
    {
        CheckConditionToView();
        
        TagManager.OnTagAdded += CheckConditionToView;
        TagManager.OnTagRemoved += CheckConditionToView;
        AcceptanceScale.OnAcceptanceScaleChanged += CheckConditionToView;
        Timer.OnTimeChanged += CheckConditionToView;
    }

    protected void OnDestroy()
    {
        TagManager.OnTagAdded -= CheckConditionToView;
        TagManager.OnTagRemoved -= CheckConditionToView;
        AcceptanceScale.OnAcceptanceScaleChanged -= CheckConditionToView;
        Timer.OnTimeChanged -= CheckConditionToView;
    }

    /// <summary>
    /// Проверка условий отображения объекта
    /// </summary>
    public void CheckConditionToView()
    {
        if (conditionToView == null)
        {
            _isViewed = true;
            this.enabled = true;
            return;
        }

        if (_isViewed && !conditionToView.CanHideAfterView)
        {
            if (!this.enabled)
            {
                this.enabled = true;
            }

            return;
        }

        bool needToShow = conditionToView.PassesTagsCondition;
        if (!conditionToView.PassesTimeCondition) needToShow = false;
        if (!conditionToView.PassesAcceptanceCondition) needToShow = false;

        if (conditionToView.NeedToHideGlobal) this.gameObject.SetActive(needToShow);
        else this.enabled = needToShow;
        
        _isViewed = needToShow;
    }

    /// <summary>
    /// Взаимодействие с интерактивным объектом
    /// </summary>
    public virtual void Interact()
    {
        OnInteracted?.Invoke(this);
        
        actionProvider?.Invoke();
        
        if (actionProvider?.ActionsCount < 1)
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

    public void ChangeBodySpriteByStage(HouseStageEnum stage)
    {
        ui.ChangeBodySpriteByStage(stage);
    }

    public void ChangeActionProvider(ListedUnityEvent actions)
    {
        actionProvider = actions;
    }

    public bool Illuminate()
    {
        if (conditionToView == null || !conditionToView.IsViewOnFlashLight) return false;
        if (IsViewed) return false;
        
        _isViewed = true;
        CheckConditionToView();
        
        return true;
    }
}