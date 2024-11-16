using UnityEngine;

public class ActionCondition : MonoBehaviour
{
    public void SetActionPlayer(string action)=> PlayerAction.PerformAction(action);
    public void ClearAction()=>PlayerAction.ClearAction();
}
