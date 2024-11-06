using UnityEngine;

public class InteractableObjectCondition : ScriptableObject
{
    [SerializeField] private bool hasAcceptanceCondition = false;
    [SerializeField, Range(0, 100)] private int minAcceptanceScale = 0;
    [SerializeField, Range(0, 100)] private int maxAcceptanceScale = 100;
    [SerializeField] private bool hasTimeCondition = false;
    [SerializeField] private int minTime;
    [SerializeField] private int maxTime;
    [SerializeField] private bool viewOnlyOnFlashlight = false;
    [SerializeField] private Tag[] hasTags;

    public bool IsViewOnlyOnFlashLight => viewOnlyOnFlashlight;

    public bool HasTagsCondition => hasTags.Length > 0;

    public bool PassesTagsCondition
    {
        get
        {
            if (!HasTagsCondition) return true;
            
            foreach (Tag tag in hasTags)
            {
                if (!TagManager.GetInstance().HasTag(tag)) return false;
            }

            return true;
        }
    }

    public bool PassesTimeCondition
    {
        get
        {
            if (!hasTimeCondition) return true;

            //TODO: условие

            return true;
        }
    }

    public bool PassesAcceptanceCondition
    {
        get
        {
            if (!hasAcceptanceCondition) return true;

            //TODO: условие

            return true;
        }
    }
}