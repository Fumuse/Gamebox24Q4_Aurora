using UnityEngine;
using UnityEngine.Localization;

public class TestCondition : MonoBehaviour
{
    /*для теста проверки условий*/
    public void SetActionPlayer(string action)=> PlayerAction.PerformAction(action);
    public void ClearAction()=>PlayerAction.ClearAction();
}
