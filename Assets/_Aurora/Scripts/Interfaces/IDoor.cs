using UnityEngine;

public interface IDoor : IInteractable
{
    public Room Room { get; }
    public IDoor ConnectedDoor { get; }
    public Transform Transform { get; }
}