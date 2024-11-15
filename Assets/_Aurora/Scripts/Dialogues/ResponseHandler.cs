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
    /// Вызывается после всех фраз, в конце всего диалога
    /// </summary>
    /// <param name="endDialogID">Id события</param>
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
    /// Вызывается в конце каждой фразы. Диалог приостанавливается пока не будет вызван метод IDialogContinue EndEvent();
    /// </summary>
    /// <param name="nameEvent"></param>
    /// <param name="handler"></param>
    private async void OnDialogEvent(string nameEvent, IDialogContinue handler)
    {
        if (_dialogueEvents == null) return;

        DialogueEvent dialogueEvent = _dialogueEvents.Find(dialogEvent=>dialogEvent.NameEvent == nameEvent);
        
        if (dialogueEvent != null)
        {
            Debug.Log("как сцена или ещё что-нибудь и при завершении продолжиться диалог");
            _dialogHandler = handler;
            await TestDelay();
            dialogueEvent.Event.Invoke();
        }

    }

    /// <summary>
    /// метод для теста. после 2 секунд будет продолжен диалог
    /// </summary>
    /// <returns></returns>
    private async UniTask TestDelay()
    {
        //что-то выполняем, показываем кат-сцену например
        await UniTask.Delay(2000);
        _dialogHandler.EndEvent();
    }


    /// <summary>
    /// При нажатии на кнопку, выбор персонажа запускаем событие.
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