using UnityEngine;

public class Door : InteractableObject, IDoor
{
    [SerializeField] private Room room;
    [SerializeField] private Door connectedDoor;

    public Room Room => room;
    public Door ConnectedDoor => connectedDoor;
    
}