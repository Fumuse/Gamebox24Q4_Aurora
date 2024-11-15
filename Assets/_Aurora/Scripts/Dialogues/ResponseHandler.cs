using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private List<ResponceEvent> _responceEvents;
    [SerializeField] private List<DialogueEvent> _dialogueEvents;
    [SerializeField] private List<DialogueEvent> _endDialogEvents;

    private IDialogContinue _dialogHandler;

    private void OnEnable()
    {
        DialogueView.ClickResponse += OnClickResponse;
        DialogueHandler.DialogEvent += OnDialogEvent;
        DialogueHandler.EndDialogEvent += OnEndDialogEvent;
    }

    private void OnDisable()
    {
        DialogueView.ClickResponse -= OnClickResponse;
        DialogueHandler.DialogEvent -= OnDialogEvent;
        DialogueHandler.EndDialogEvent -= OnEndDialogEvent;
    }

    /// <summary>
    /// ���������� ����� ���� ����, � ����� ����� �������
    /// </summary>
    /// <param name="endDialogID">Id �������</param>
    private void OnEndDialogEvent(string endDialogID)
    {
        if (_endDialogEvents == null) return;

        DialogueEvent dialogueEvent = _endDialogEvents.Find(dialogEvent => dialogEvent.NameEvent == endDialogID);
        Debug.Log($"{endDialogID}");


        if (dialogueEvent != null)
        {
            dialogueEvent.Event.Invoke();
        }

    }

    /// <summary>
    /// ���������� � ����� ������ �����. ������ ������������������ ���� �� ����� ������ ����� IDialogContinue EndEvent();
    /// </summary>
    /// <param name="nameEvent"></param>
    /// <param name="handler"></param>
    private async void OnDialogEvent(string nameEvent, IDialogContinue handler)
    {
        if (_dialogueEvents == null) return;

        DialogueEvent dialogueEvent = _dialogueEvents.Find(dialogEvent=>dialogEvent.NameEvent == nameEvent);
        
        if (dialogueEvent != null)
        {
            Debug.Log("��� ����� ��� ��� ���-������ � ��� ���������� ������������ ������");
            _dialogHandler = handler;
            await TestDelay();
            dialogueEvent.Event.Invoke();
        }

    }

    /// <summary>
    /// ����� ��� �����. ����� 2 ������ ����� ��������� ������
    /// </summary>
    /// <returns></returns>
    private async UniTask TestDelay()
    {
        //���-�� ���������, ���������� ���-����� ��������
        await UniTask.Delay(2000);
        _dialogHandler.EndEvent();
    }


    /// <summary>
    /// ��� ������� �� ������, ����� ��������� ��������� �������.
    /// </summary>
    /// <param name="responseID"></param>
    private void OnClickResponse(int responseID)
    {
        if (_responceEvents == null) return;

        ResponceEvent responceEvent = _responceEvents.Find(responseEvent => responseEvent.ResponceID == responseID);
          
        if (responceEvent != null)
        {
           responceEvent.Event.Invoke();
         }
   
    }
}

[System.Serializable]
public class DialogueEvent
{
    public string NameEvent;
    public UnityEvent Event;
}

[System.Serializable]
public class ResponceEvent
{
    public string NameEvent;
    public int ResponceID;
    public UnityEvent Event;
}