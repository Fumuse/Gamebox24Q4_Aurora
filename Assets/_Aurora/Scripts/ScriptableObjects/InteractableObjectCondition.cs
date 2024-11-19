using UnityEngine;

[CreateAssetMenu(fileName = "InteractableObjectsCondition", menuName = "Conditions/InteractableObjectsCondition")]
public class InteractableObjectCondition : ObjectCondition
{
    [SerializeField] protected bool viewOnFlashlight = false;
    public bool IsViewOnFlashLight => viewOnFlashlight;

    [SerializeField] protected bool canHideAfterView = false;
    public bool CanHideAfterView => canHideAfterView;

    [SerializeField] protected bool needToHideGlobal = false;
    public bool NeedToHideGlobal => needToHideGlobal;

    [SerializeField] protected bool needToHideInTutorial = false;
    public bool NeedToHideInTutorial => needToHideInTutorial;
}