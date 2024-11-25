using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueView : MonoBehaviour
{
    public static event Action<DialogueEventsTypeEnum> ClickResponse;
    public static event Action<DialogueNode> NextResponseDialog;

    [SerializeField] private TMP_Text dialogueTMP;
    [SerializeField] private Canvas canvas;
    [SerializeField] private bool useAnimationText;
    [SerializeField] private int timeDelayAnimationText;
    [SerializeField] private bool isClickResponse;
    [SerializeField] private Button closeDialogueButton;
    [SerializeField] private Image containerToShowSomeImage;

    [field: SerializeField] public Button[] Buttons { get; private set; }
    private TMP_Text[] _buttonsTmp;
    private CancellationTokenSource _cts = new();

    private string _animationText;
    private bool _isSkipDialog;
    private Dialogue _dialogue;

    private string _action;
    private string _responseText;
    private string _dialogueText;
    private string _actorName;
    private bool _isButtonView;

    #region Show Some Image Action
    private bool _needToShowSomeImage = false;
    private Sprite _someImageSpriteFromResponse;
    #endregion

    public bool IsCloseButtonClicked { get; private set; } = false;

    private void Start()
    {
        if (Buttons.Length > 0)
        {
            _buttonsTmp = new TMP_Text[Buttons.Length];
            for (int i = 0; i < Buttons.Length; i++)
            {
                _buttonsTmp[i] = Buttons[i].GetComponentInChildren<TMP_Text>();
            }
        }
    }

    public void Show()
    {
        canvas.enabled = true;
    }

    public void Hide()
    {
        canvas.enabled = false;
        UnsubscribeFromEvents();
    }

    private void OnEnable()
    {
        _cts = new();
    }

    private void OnDisable()
    {
        _cts?.Cancel();
    }

    delegate void ResponseMessage();

    public async UniTask SetText(Dialogue dialogue)
    {
        _dialogue = dialogue;

        SubscribingToEvents();
        ShowSomeImageContainer();

        _dialogueText = _dialogue.DialogueText.GetLocalizedString();
        _actorName = !_dialogue.Tag.IsEmpty ? _dialogue.Tag.GetLocalizedString() : null;

        isClickResponse = false;
        _isSkipDialog = false;
        
        _animationText = "";
        if (_actorName != null) _animationText += $"<color=yellow>{_actorName}</color>: ";

        if (useAnimationText)
        {
            foreach (char ch in _dialogueText)
            {
                _animationText += ch;
                dialogueTMP.text = _animationText;

                if (_isSkipDialog)
                {
                    _animationText = _actorName != null ? $"<color=yellow>{_actorName}</color>: " : "";
                    _animationText += $"{_dialogueText}";
                    
                    dialogueTMP.text = _animationText;
                    return;
                }

                await UniTask.Delay(timeDelayAnimationText);
            }
        }
        else
        {
            _animationText += _dialogueText;
            dialogueTMP.text = _animationText;
        }
    }

    private void SubscribingToEvents()
    {
        _dialogue.DialogueText.StringChanged += OnDialogueStringChange;
        _dialogue.Tag.StringChanged += OnTagStringChange;

        if (_dialogue.Response.Length > 0)
        {
            for (int i = 0; i < _dialogue.Response.Length; i++)
            {
                bool isResponse = CanDisplayResponse(_dialogue.Response[i]);
                if (isResponse)
                {
                    int index = i;
                    _dialogue.Response[i].ResponseText.StringChanged += (text) => OnStringResponseChange(text, index);
                    _dialogue.Response[i].imageToShowAfterResponse.AssetChanged += OnSpriteResponseChange;
                }
            }
        }
    }

    private void UnsubscribeFromEvents()
    {
        _dialogue.DialogueText.StringChanged -= OnDialogueStringChange;
        _dialogue.Tag.StringChanged -= OnTagStringChange;

        if (_dialogue.Response.Length > 0)
        {
            for (int i = 0; i < _dialogue.Response.Length; i++)
            {
                bool isResponse = CanDisplayResponse(_dialogue.Response[i]);

                if (isResponse)
                {
                    _dialogue.Response[i].ResponseText.StringChanged -= (text) => OnStringResponseChange(text, i);
                    _dialogue.Response[i].imageToShowAfterResponse.AssetChanged -= OnSpriteResponseChange;
                }
            }
        }
    }

    private void OnTagStringChange(string tag)
    {
        _actorName = tag;
    }

    private void OnDialogueStringChange(string dialogueText)
    {
        string dialogueUpdatedText = "";
        if (_actorName != null) dialogueUpdatedText += $"<color=yellow>{_actorName}: </color> ";
        dialogueUpdatedText += dialogueText;
        
        dialogueTMP.text = dialogueUpdatedText;
    }

    private void OnStringResponseChange(string value, int index)
    {
        if (_dialogue.Response.Length == 0) return;

        if (index >= _dialogue.Response.Length) return;

        _buttonsTmp[index].text = value;
    }

    private void OnSpriteResponseChange(Sprite sprite)
    {
        if (containerToShowSomeImage == null) return;
        containerToShowSomeImage.sprite = sprite;
        _someImageSpriteFromResponse = sprite;
    }

    public async UniTask WaitResponse(Dialogue dialogue)
    {
        ClearViewResponses();
        ButtonResponseInitialize(dialogue);

        if (dialogue.Response.Length > 0 && _isButtonView)
        {
            bool isCanceled = await UniTask.WaitUntil(() => isClickResponse, cancellationToken: _cts.Token)
                .SuppressCancellationThrow();
            if (isCanceled) return;
        }
    }

    public void SetSkipDialog(bool stopDialog) => _isSkipDialog = stopDialog;

    public void ClearText()
    {
        dialogueTMP.text = "";
    }

    private bool CanDisplayResponse(Response response)
    {
        if (response.Condition == null) return true;

        bool needToShow = response.Condition.PassesTagsCondition;
        if (!response.Condition.PassesTimeCondition) needToShow = false;
        if (!response.Condition.PassesAcceptanceCondition) needToShow = false;

        return needToShow;
    }

    private void ButtonResponseInitialize(Dialogue dialog)
    {
        isClickResponse = false;

        if (closeDialogueButton != null)
        {
            closeDialogueButton.gameObject.SetActive(false);
        }
        
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        for (int i = 0; i < Buttons.Length; i++)
        {
            if (i < dialog.Response.Length)
            {
                bool isResponse = CanDisplayResponse(dialog.Response[i]);
                if (isResponse)
                {
                    if (_isButtonView == false)
                    {
                        _isButtonView = true;
                    }

                    Response response = dialog.Response[i];

                    _responseText = response.ResponseText.GetLocalizedString();
                    _buttonsTmp[i].text = _responseText;
                    Buttons[i].gameObject.SetActive(true);

                    DialogueEventsTypeEnum responseEventType = response.EventType;
                    DialogueNode nextDialog = response.NextDialogue;

                    Buttons[i].onClick.AddListener(() => OnResponseSelected(responseEventType, response, nextDialog));
                }
            }
            else
            {
                Buttons[i].gameObject.SetActive(false);
            }
        }
    }

    public void ShowCloseDialogueButton()
    {
        if (closeDialogueButton == null) return;

        IsCloseButtonClicked = false;
        closeDialogueButton.onClick.RemoveAllListeners();
        closeDialogueButton.onClick.AddListener(() => IsCloseButtonClicked = true);
        closeDialogueButton.gameObject.SetActive(true);
    }

    public void ClearViewResponses()
    {
        _isButtonView = false;

        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].onClick.RemoveAllListeners();
            Buttons[i].gameObject.SetActive(false);
        }
    }

    private void HideButtons()
    {
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].gameObject.SetActive(false);
        }
    }

    private void OnResponseSelected(DialogueEventsTypeEnum responseType, Response response, DialogueNode nextDialog)
    {
        // string responseAction = response.ResponseText.GetLocalizedString();
        // PlayerAction.PerformAction(responseAction);

        isClickResponse = true;
        HideButtons();

        _someImageSpriteFromResponse = !response.imageToShowAfterResponse.IsEmpty ? 
            response.imageToShowAfterResponse.LoadAsset() : null;
        
        ClickResponse?.Invoke(responseType);

        if (response.TagsToAddAfterAction.Length > 0)
        {
            foreach (Tag tag in response.TagsToAddAfterAction)
            {
                GameManager.Instance.TagManager.AddTag(tag);
            }
        }

        if (response.AcceptanceScaleCost > 0)
        {
            GameManager.Instance.AcceptanceScale.SpentAcceptance(response.AcceptanceScaleCost);
        }

        if (response.TimeCost > 0)
        {
            GameManager.Instance.Timer.SpendTime(response.TimeCost);
        }

        if (nextDialog)
        {
            NextResponseDialog?.Invoke(nextDialog);
        }
    }

    public void TriggerSomeImageAction()
    {
        if (containerToShowSomeImage == null) return;

        _needToShowSomeImage = true;
    }

    public void ChangeSomeImageAction(Sprite sprite)
    {
        if (containerToShowSomeImage == null) return;
        TriggerSomeImageAction();

        _needToShowSomeImage = true;
        _someImageSpriteFromResponse = sprite;
    }

    private void ShowSomeImageContainer()
    {
        if (containerToShowSomeImage == null) return;
        ClearSomeImageContainer();

        if (_needToShowSomeImage)
        {
            if (_someImageSpriteFromResponse != null)
            {
                containerToShowSomeImage.sprite = _someImageSpriteFromResponse;
                containerToShowSomeImage.enabled = true;
            }
            _needToShowSomeImage = false;
        }
    }

    private void ClearSomeImageContainer()
    {
        if (containerToShowSomeImage == null) return;
        containerToShowSomeImage.enabled = false;
    }
}