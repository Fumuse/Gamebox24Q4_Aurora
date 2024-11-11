using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStateMachine : StateMachine
{
    [SerializeField] private List<ActionSettingsKeyPair> settingsMap;
    [SerializeField] private List<DoorKeyPair> doorsMap;
    [SerializeField] private List<InteractableObjectsKeyPair> objectsMap;
    [SerializeField] private CanvasGroup overlayWrapper;

    public IReadOnlyList<ActionSettingsKeyPair> SettingsMap => settingsMap;
    public IReadOnlyList<InteractableObjectsKeyPair> ObjectsMap => objectsMap;
    public List<DoorKeyPair> DoorsMap => doorsMap;
    public CanvasGroup OverlayWrapper => overlayWrapper;

    private void OnValidate()
    {
        if (doorsMap.Count < 1)
        {
            Door[] doors = FindObjectsByType<Door>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (Door door in doors)
            {
                DoorKeyPair pair = new DoorKeyPair
                {
                    door = door,
                    doorKey = door.name
                };

                doorsMap.Add(pair);
            }
        }
    }

    public void StartTutorial()
    {
        LockAllDoors();
        SwitchState(new StartMovieState(this));
    }

    private void LockAllDoors()
    {
        foreach (DoorKeyPair pair in doorsMap)
        {
            pair.door.gameObject.SetActive(false);
        }
    }

    public void EndTutorial()
    {
        foreach(DoorKeyPair door in doorsMap)
        {
            door.door.gameObject.SetActive(true);
        }
    }
}