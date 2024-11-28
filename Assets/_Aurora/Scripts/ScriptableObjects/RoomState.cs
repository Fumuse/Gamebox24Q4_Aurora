using UnityEngine;

[CreateAssetMenu(fileName = "RoomState", menuName = "States/RoomState")]
public class RoomState : ScriptableObject
{
    [SerializeField] private Sprite light;
    [SerializeField] private Sprite dark;
    [SerializeField] private Sprite broken;

    public Sprite Light => light;
    public Sprite Dark => dark;
    public Sprite Broken => broken;
}