using UnityEngine;

public class Door : InteractableObject, IDoor
{
    [SerializeField] private Room room;
    [SerializeField] private Door connectedDoor;

    private Door _realConnectedDoor;

    public Room Room => room;
    public IDoor ConnectedDoor => connectedDoor;
    public Transform Transform => objectPosition;

    public override void Init()
    {
        base.Init();

        _realConnectedDoor = connectedDoor;
        _isViewed = true;
    }

    public void ChangeConnectedDoor(Door door)
    {
        connectedDoor = door;
    }

    public void ReturnRealConnectedDoor()
    {
        connectedDoor = _realConnectedDoor;
    }
}