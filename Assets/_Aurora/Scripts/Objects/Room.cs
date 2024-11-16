using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private RoomState stateSprites;
    [SerializeField, Tooltip("Нижний слой")] private SpriteRenderer background;
    [SerializeField, Tooltip("Верхний слой")] private SpriteRenderer foreground;
    [SerializeField] private Door[] doors;
    [SerializeField] private InteractableObject[] interactableObjects;
    [SerializeField] private RoomAdditionalObject[] additionalObjects;

    public IReadOnlyList<Door> Doors => doors;
    public IReadOnlyList<InteractableObject> InteractableObjects => interactableObjects;

    private void OnValidate()
    {
        InteractableObject[] allInteractableObjects = GetComponentsInChildren<InteractableObject>();

        interactableObjects = allInteractableObjects.Where(obj => !(obj is Door)).ToArray();
        doors = GetComponentsInChildren<Door>();
        additionalObjects = GetComponentsInChildren<RoomAdditionalObject>();
    }

    public void ChangeSpriteStage(HouseStageEnum stage)
    {
        if (stateSprites != null)
        {
            background.sprite = stateSprites.Light;

            if (foreground != null)
            {
                if (stage == HouseStageEnum.Light && stateSprites.Light != null)
                    foreground.sprite = stateSprites.Light;
        
                if (stage == HouseStageEnum.Dark && stateSprites.Dark != null)
                    foreground.sprite = stateSprites.Dark;
        
                if (stage == HouseStageEnum.Broken && stateSprites.Broken != null)
                    foreground.sprite = stateSprites.Broken;
            }
        }

        foreach (InteractableObject intObject in interactableObjects)
        {
            intObject.ChangeBodySpriteByStage(stage);
        }

        foreach (RoomAdditionalObject additionalObject in additionalObjects)
        {
            additionalObject.ChangeBodySpriteByStage(stage);
        }
    }
}