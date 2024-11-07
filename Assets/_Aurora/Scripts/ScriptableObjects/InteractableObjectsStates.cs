using UnityEngine;

[CreateAssetMenu(fileName = "InteractableObjectStates", menuName = "InteractableObjects/InteractableObjectsStates")]
public class InteractableObjectsStates : ScriptableObject
{
    [SerializeField] private Sprite light;
    [SerializeField] private Sprite darkness;
    [SerializeField] private Sprite broken;

    public Sprite Light => light;
    public Sprite Darkness => darkness;
    public Sprite Broken => broken;
}