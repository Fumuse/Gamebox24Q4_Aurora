using UnityEngine;

[CreateAssetMenu(fileName = "InteractableObjectState", menuName = "States/InteractableObjectState")]
public class InteractableObjectState : ScriptableObject
{
    [SerializeField] private Sprite light;
    [SerializeField] private Sprite dark;
    [SerializeField] private Sprite broken;

    public Sprite Light => light;
    public Sprite Dark => dark;
    public Sprite Broken => broken;
}