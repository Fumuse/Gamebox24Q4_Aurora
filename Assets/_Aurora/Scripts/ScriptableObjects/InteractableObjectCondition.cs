using UnityEngine;

[CreateAssetMenu(fileName = "InteractableObjectsCondition", menuName = "Conditions/InteractableObjectsCondition")]
public class InteractableObjectCondition : ObjectCondition
{
    [SerializeField] protected bool viewOnlyOnFlashlight = false;
    public bool IsViewOnlyOnFlashLight => viewOnlyOnFlashlight;
}