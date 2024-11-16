using UnityEngine;

public abstract class ObjectCondition : ScriptableObject
{
    [SerializeField] protected bool hasAcceptanceCondition = false;
    [SerializeField, Range(0, 100)] protected int minAcceptanceScale = 0;
    [SerializeField, Range(0, 100)] protected int maxAcceptanceScale = 100;
    [SerializeField] protected bool hasTimeCondition = false;
    [SerializeField] protected int minTime;
    [SerializeField] protected int maxTime;
    [SerializeField, Tooltip("Проверка на то, чтобы у игрока были какие-то теги")] protected Tag[] hasTags;
    [SerializeField, Tooltip("Проверка на то, чтобы у игрока не было каких-то тегов")] protected Tag[] hasntTags;

    public bool HasTagsCondition => hasTags.Length > 0 || hasntTags.Length > 0;

    public bool PassesTagsCondition
    {
        get
        {
            if (!HasTagsCondition) return true;
            
            foreach (Tag tag in hasTags)
            {
                if (!GameManager.Instance.TagManager.HasTag(tag)) return false;
            }
            
            foreach (Tag tag in hasntTags)
            {
                if (GameManager.Instance.TagManager.HasTag(tag)) return false;
            }

            return true;
        }
    }

    public bool PassesTimeCondition
    {
        get
        {
            if (!hasTimeCondition) return true;

            if (GameManager.Instance.Timer.TimeToEnd < minTime)
                return false;
            
            if (GameManager.Instance.Timer.TimeToEnd > maxTime)
                return false;

            return true;
        }
    }

    public bool PassesAcceptanceCondition
    {
        get
        {
            if (!hasAcceptanceCondition) return true;

            if (GameManager.Instance.AcceptanceScale.Current < minAcceptanceScale)
                return false;
            if (GameManager.Instance.AcceptanceScale.Current > maxAcceptanceScale)
                return false;

            return true;
        }
    }
}