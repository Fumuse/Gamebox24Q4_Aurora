using UnityEngine;

[CreateAssetMenu(fileName = "PlayerState", menuName = "States/PlayerState")]
public class PlayerState : ScriptableObject
{
    [SerializeField] private Sprite light;
    [SerializeField] private Sprite dark;
    [SerializeField] private Sprite broken;

    public Sprite Light => light;
    public Sprite Dark => dark;
    public Sprite Broken => broken;
}