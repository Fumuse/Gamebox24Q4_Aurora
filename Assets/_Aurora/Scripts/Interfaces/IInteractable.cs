using UnityEngine;

public interface IInteractable
{
    public bool IsInteracted { get; }
    public Vector3 Position { get; }
    public float Offset { get; }
    public GameObject GameObject { get; }
    
    public void PreInteract();
    public void Interact();
    public void DeclineInteract();
    public void FinishInteract();
    
    public void ChangeActionProvider(ListedUnityEvent actions);
}