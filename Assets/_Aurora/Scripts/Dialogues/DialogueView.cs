using Cysharp.Threading.Tasks;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class DialogueView : MonoBehaviour
{
    public static event Action<int> ClickResponse;
    public static event Action<DialogueNode> NextResponseDialog;

    [SerializeField] private TMP_Text _dialogueTMP;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private bool _useAnimationText;
    [SerializeField] private int _timeDelayAnimationText;

    [field:SerializeField] public Button[] Buttons { get; private set; }

    private string animationText;
    private bool _isSkipDialog;
    public bool IsClickResponse;
    private Dialogue _dialogue;

    private string _action;
    private string _responseText;
    private string _dialogueText;
    private string _tag;
    private bool _isButtonView;

    public void Show()
    { 
      _canvas.enabled = true;
    }

    public void Hide()
    {
       _canvas.enabled = false;
        UnsubscribeFromEvents();
    }

    delegate void ResponseMessage();

    public async UniTask SetText(Dialogue dialogue)
    {
        _dialogue = dialogue;

        SubscribingToEvents();

        _dialogueText = _dialogue.DialogueText.GetLocalizedString();
        _tag = _dialogue.Tag.GetLocalizedString();

        IsClickResponse = false;
        _isSkipDialog = false;
        animationText = $"<color=yellow>{_tag}: </color>";

        if (_useAnimationText)
        {
            foreach (char ch in _dialogueText)
            {
                animationText += ch;
                _dialogueTMP.text = animationText;

                 if(_isSkipDialog)
                 {
                     animationText = $"<color=yellow>{_tag}: </color> {_dialogueText}" ;
                    _dialogueTMP.text = animationText;
                     return;
                 }

                await UniTask.Delay(_timeDelayAnimationText);
            }
        }
        else
        {
            animationText += _dialogueText;
            _dialogueTMP.text = animationText;
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
                bool isResponce = CanDisplayResponce(_dialogue.Response[i]);
                if (isResponce)
                {
                    int index = i;
                    _dialogue.Response[i].ResponseText.StringChanged += (text) => OnStringResponseChange(text, index);
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
                bool isResponce = CanDisplayResponce(_dialogue.Response[i]);
                
                if (isResponce)
                {
                    _dialogue.Response[i].ResponseText.StringChanged -= (text) => OnStringResponseChange(text, i);
                }
            }
        }
    }

    private void OnTagStringChange(string tag)
    {
       _tag = tag;
    }

    private void OnDialogueStringChange(string dialogueText)
    {
        _dialogueTMP.text = $"<color=yellow>{_tag}: </color> {dialogueText}"; ;
    }

    public async UniTask WaitResponse(Dialogue dialogue)
    {
        ClearViewResponces();
        ButtonResponseInitialize(dialogue);

        if (dialogue.Response.Length > 0 && _isButtonView)
        {
           await WaitResponse();
        }
    }

    public void SetSkipDialog(bool stopDialog) => _isSkipDialog = stopDialog;
 
    private async UniTask WaitResponse()
    {
        while (true)
        {
            if (IsClickResponse)
            {

                return;
            }

            await UniTask.Yield();
        }
    }

    public void ClearText()
    {
        _dialogueTMP.text = "";
    }

    private bool CanDisplayResponce(Response responce)
    {
        if(responce.Condition.Count == 0) return true;

        bool isCondition = false;
        int count = responce.Condition.FindAll(cond => cond.Required == PlayerAction.HasAction(cond.Action)).Count;

        if (count == responce.Condition.Count) isCondition = true;

        return isCondition;
    }

    private void ButtonResponseInitialize(Dialogue dialog)
    {
        IsClickResponse = false;

        for (int i = 0; i < Buttons.Length; i++)
        {
            if (i < dialog.Response.Length)
            {
                bool isResponce = CanDisplayResponce(dialog.Response[i]);
                if (isResponce)
                {
                    if (_isButtonView == false)
                    {
                        _isButtonView = true;
                    }

                    string colorTag = dialog.Response[i].ColorText;

                    _responseText = dialog.Response[i].ResponseText.GetLocalizedString();

                    Buttons[i].GetComponentInChildren<TMP_Text>().text = colorTag != "" ? $"<color={colorTag}>{_responseText}</color>" : _responseText;
                    Buttons[i].gameObject.SetActive(true);

                    int responseID = dialog.Response[i].ID;
                    DialogueNode nextDialog = dialog.Response[i].NextDialogue;
                    string responce = dialog.Response[i].ResponseText.GetLocalizedString();
                    
                    Buttons[i].onClick.AddListener(() => OnResponseSelected(responseID, responce, nextDialog));
                }
            }
            else
            {
                Buttons[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnStringResponseChange(string value, int index)
    {
        if (_dialogue.Response.Length == 0) return;

        if(index >= _dialogue.Response.Length) return;

        string colorTag = _dialogue.Response[index].ColorText;
        Buttons[index].GetComponentInChildren<TMP_Text>().text = colorTag != "" ? $"<color={colorTag}>{value}</color>" : value;
    }

    public void ClearViewResponces()
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

    private void OnResponseSelected(int responseID, string responce, DialogueNode nextDialog)
    {
        PlayerAction.PerformAction(responce);

        IsClickResponse = true;
        HideButtons();

        ClickResponse?.Invoke(responseID);

        if (nextDialog)
        {
            NextResponseDialog?.Invoke(nextDialog);
        }
        
    }

    
}