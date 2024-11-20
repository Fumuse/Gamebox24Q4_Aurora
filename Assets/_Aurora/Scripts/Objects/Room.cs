using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Room : MonoBehaviour
{
    [SerializeField] private RoomState stateSprites;
    [SerializeField, Tooltip("Нижний слой")] private SpriteRenderer background;
    [SerializeField, Tooltip("Верхний слой (темнота)")] private RoomShadow shadow;
    [SerializeField] private Door[] doors;
    [SerializeField] private InteractableObject[] interactableObjects;
    [SerializeField] private RoomAdditionalObject[] additionalObjects;
    [SerializeField] private Light2D[] lights;
    [SerializeField] private AmbienceAnimation[] ambience;

    public IReadOnlyList<Door> Doors => doors;
    public IReadOnlyList<InteractableObject> InteractableObjects => interactableObjects;
    public RoomShadow Shadow => shadow;
    public AmbienceAnimation[] Ambience => ambience;

    private void OnValidate()
    {
        InteractableObject[] allInteractableObjects = GetComponentsInChildren<InteractableObject>();

        interactableObjects = allInteractableObjects.Where(obj => !(obj is Door)).ToArray();
        doors = GetComponentsInChildren<Door>();
        additionalObjects = GetComponentsInChildren<RoomAdditionalObject>();
        shadow ??= GetComponentInChildren<RoomShadow>();

        if (lights.Length < 1)
        {
            lights ??= GetComponentsInChildren<Light2D>(); 
        }
        
        ambience ??= GetComponentsInChildren<AmbienceAnimation>(true); 
    }

    public void ChangeSpriteStage(HouseStageEnum stage)
    {
        if (stateSprites != null)
        {
            if (background != null)
            {
                if (stage == HouseStageEnum.Light && stateSprites.Light != null)
                    background.sprite = stateSprites.Light;
        
                if (stage == HouseStageEnum.Dark && stateSprites.Dark != null)
                    background.sprite = stateSprites.Dark;
        
                if (stage == HouseStageEnum.Broken && stateSprites.Broken != null)
                    background.sprite = stateSprites.Broken;
            }
        }

        shadow.Change(stage);

        foreach (InteractableObject intObject in interactableObjects)
        {
            intObject.ChangeBodySpriteByStage(stage);
        }

        foreach (Door door in doors)
        {
            door.ChangeBodySpriteByStage(stage);
        }

        foreach (RoomAdditionalObject additionalObject in additionalObjects)
        {
            additionalObject.ChangeBodySpriteByStage(stage);
        }

        foreach (Light2D roomLight in lights)
        {
            roomLight.enabled = stage == HouseStageEnum.Light;
        }

        foreach (AmbienceAnimation animation in ambience)
        {
            animation.ShowSprite();
        }
    }
}