using System;
using System.Collections.Generic;
using System.Linq;

public class TimeEndedState : State
{
    private EndGameStateMachine _stateMachine;
    private GameManager _manager;
    private TeleportProvider _teleportProvider;
    private WhisperProvider _whisperProvider;

    public TimeEndedState(EndGameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public override void Enter()
    {
        _manager = GameManager.Instance;
        _teleportProvider = GameProvidersManager.Instance.TeleportProvider;
        _whisperProvider = GameProvidersManager.Instance.WhisperProvider;

        Door door = _stateMachine.GetDoorByKey("Room_3_DoorLeft");
        if (door == null) return;

        _stateMachine.LockAllDoors();
        _teleportProvider.TeleportToConnectedRoom(door);

        LockObjects(door.ConnectedDoor.Room.InteractableObjects);

        IInteractable photoAlbum = _stateMachine.GetInteractableByKey("Room_4_PhotoAlbum");
        if (photoAlbum == null) return;

        photoAlbum.ForceEnableObject();
    }

    public override void Tick()
    {
    }

    public override void Exit()
    {
    }

    private void LockObjects(IReadOnlyList<InteractableObject> interactableObjects)
    {
        foreach (InteractableObject interactableObject in interactableObjects)
        {
            InteractableObject.BlockInteractedObject(interactableObject);
        }
    }
}