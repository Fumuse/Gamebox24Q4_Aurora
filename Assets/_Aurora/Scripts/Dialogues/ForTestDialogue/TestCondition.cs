using UnityEngine;
using UnityEngine.Localization;

public class TestCondition : MonoBehaviour
{
    /*��� ����� �������� �������*/
    public void SetActionPlayer(string action)=> PlayerAction.PerformAction(action);
    public void ClearAction()=>PlayerAction.ClearAction();
}
