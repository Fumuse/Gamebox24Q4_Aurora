using System;
using UnityEngine;

[RequireComponent(typeof(InteractableObjectUI))]
public class InteractableObject : MonoBehaviour, IInteractable, IIlluminated
{
    [Header("Настройки для отображения и взаимодействия")]
    [SerializeField] protected InteractableObjectCondition conditionToView;
    [SerializeField] protected float positionOffset;
    [SerializeField] protected int clickSort = 100;
    [SerializeField] protected ListedUnityEvent actionProvider;
    
    [Header("Подключение зависимостей")]
    [SerializeField] protected Transform objectPosition;
    [SerializeField] private InteractableObjectUI ui;

    public bool IsInteractBlocked { get; private set; }
    public bool IsInteracted { get; private set; }
    public Vector3 Position => objectPosition.position;
    public float Offset => positionOffset;

    public GameObject GameObject => this.gameObject;
    
    public int ClickSort => clickSort;

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

    public bool IsViewed
    {
        get => _isViewed;
        private set
        {
            _isViewed = value;
            
            //TODO: это, в теории, можно сделать по-другому
            if (_isViewed)
            {
                ui.ChangeVisionState(InteractableStateVisionEnum.Viewed);
            }
            else
            {
                ui.ChangeVisionState(InteractableStateVisionEnum.Default);
            }
        }
    }

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
        GameManager.OnTutorialStateChanged += CheckConditionToView;
    }

    protected void OnDestroy()
    {
        TagManager.OnTagAdded -= CheckConditionToView;
        TagManager.OnTagRemoved -= CheckConditionToView;
        AcceptanceScale.OnAcceptanceScaleChanged -= CheckConditionToView;
        Timer.OnTimeChanged -= CheckConditionToView;
        GameManager.OnTutorialStateChanged -= CheckConditionToView;
    }

    /// <summary>
    /// Проверка условий отображения объекта
    /// </summary>
    public void CheckConditionToView()
    {
        if (conditionToView == null)
        {
            IsViewed = true;
            this.enabled = true;
            return;
        }

        if (IsViewed && !conditionToView.CanHideAfterView)
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

        if (conditionToView.NeedToHideGlobal)
        {
            this.gameObject.SetActive(needToShow);
        }
        else
        {
            this.enabled = needToShow;
        }

        if (conditionToView.NeedToHideInTutorial)
        {
            ui.BodySprite.enabled = !GameManager.Instance.TutorialStage;
        }
        
        IsViewed = needToShow;
    }

    /// <summary>
    /// Взаимодействие с интерактивным объектом
    /// </summary>
    public virtual void Interact()
    {
        Debug.Log($"Start interact {Time.time}");
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

    public void BlockInteract()
    {
        IsInteractBlocked = true;
        ChangeBodySpriteByStage(GameManager.Instance.CurrentStage);
    }

    public void UnblockInteract()
    {
        IsInteractBlocked = false;
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
        
        IsViewed = true;
        CheckConditionToView();
        
        return true;
    }

    public static void BlockInteractedObject(IInteractable interactable)
    {
        interactable.BlockInteract();
    }

    public static void UnblockInteractedObject(IInteractable interactable)
    {
        interactable.UnblockInteract();
    }
}