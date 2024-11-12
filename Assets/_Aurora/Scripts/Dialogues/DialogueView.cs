using Cysharp.Threading.Tasks;
using System;
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

    public void Show()
    { 
      _canvas.enabled = true;
    }

    public void Hide()
    {
       _canvas.enabled = false;
    }

    public async UniTask SetText(Dialogue dialogue)
    {
        _dialogue = dialogue;

        _dialogueText = _dialogue.DialogueText;
        _tag = _dialogue.Tag;

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

    public async UniTask WaitResponse(Dialogue dialogue)
    {
        ClearViewResponces();
        ButtonResponseInitialize(dialogue);

        if (dialogue.Response.Length > 0)
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
        foreach (Condition condition in responce.Condition) 
        {
            bool actionState = PlayerAction.HasAction(condition.Action);

            if (actionState != condition.Required)
            {
                return false;
            }
        }

        return true;
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
                    string colorTag = dialog.Response[i].ColorText;
                    _responseText = dialog.Response[i].ResponseText;

                    Buttons[i].GetComponentInChildren<TMP_Text>().text = colorTag != "" ? $"<color={colorTag}>{_responseText}</color>" : _responseText;
                    Buttons[i].gameObject.SetActive(true);

                    int responseID = dialog.Response[i].ID;
                    DialogueNode nextDialog = dialog.Response[i].NextDialogue;
                    string responce = dialog.Response[i].ResponseText;
                    Buttons[i].onClick.AddListener(() => OnResponseSelected(responseID, responce, nextDialog));
                }
            }
            else
            {
                Buttons[i].gameObject.SetActive(false);
            }
        }
    }

    public void ClearViewResponces()
    {
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
