using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public abstract class TutorialBaseState : State
{
    protected readonly TutorialStateMachine stateMachine;
    
    protected TutorialBaseState(TutorialStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected Door GetDoorByKey(string doorKey)
    {
        foreach (DoorKeyPair pair in stateMachine.DoorsMap)
        {
            if (pair.doorKey != doorKey) continue;
            return pair.door;
        }

        return null;
    }
    
    protected void LockDoor(string doorKey)
    {
        foreach (DoorKeyPair pair in stateMachine.DoorsMap)
        {
            if (pair.doorKey != doorKey) continue;
            pair.door.gameObject.SetActive(false);
        }
    }

    protected void UnlockDoor(string doorKey)
    {
        foreach (DoorKeyPair pair in stateMachine.DoorsMap)
        {
            if (pair.doorKey != doorKey) continue;
            pair.door.gameObject.SetActive(true);
        }
    }

    protected void ChangeDoorConnectedDoor(string doorKeyToChange, string newConnectedDoorKey)
    {
        Door doorToChange = null;
        Door doorToConnect = null;
        foreach (DoorKeyPair pair in stateMachine.DoorsMap)
        {
            if (pair.doorKey == doorKeyToChange)
            {
                doorToChange = pair.door;
            }
            if (pair.doorKey == newConnectedDoorKey)
            {
                doorToConnect = pair.door;
            }
        }

        if (doorToChange != null && doorToConnect != null)
        {
            doorToChange.ChangeConnectedDoor(doorToConnect);
        }
    }

    protected ActionSettings GetSettingByKey(string key)
    {
        ActionSettingsKeyPair setting = stateMachine.SettingsMap.FirstOrDefault(
            (setting) => setting.key == key
        );
        
        if (setting == null)
        {
            throw new Exception($"{key} settings not founded.");
        }

        return setting.actionSetting;
    }

    protected IInteractable GetInteractableByKey(string key)
    {
        InteractableObjectsKeyPair setting = stateMachine.ObjectsMap.FirstOrDefault(
            (setting) => setting.key == key
        );
        
        if (setting == null)
        {
            throw new Exception($"{key} interactable object not founded.");
        }

        return setting.interactable;
    }
}