using UnityEngine;

public class RoomAdditionalObject : MonoBehaviour
{
    [Header("Objects sprites controller")] 
    [SerializeField] private SpriteRenderer bodySprite;
    [SerializeField] private InteractableObjectState states;

    private void OnValidate()
    {
        bodySprite ??= GetComponentInChildren<SpriteRenderer>();
    }

    public void ChangeBodySpriteByStage(HouseStageEnum stage)
    {
        if (states == null) return;
        
        if (stage == HouseStageEnum.Light && states.Light != null)
            bodySprite.sprite = states.Light;
        if (stage == HouseStageEnum.Dark && states.Dark != null)
            bodySprite.sprite = states.Dark;
        if (stage == HouseStageEnum.Broken && states.Broken != null)
            bodySprite.sprite = states.Broken;
    }
}