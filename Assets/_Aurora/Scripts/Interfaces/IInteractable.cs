using UnityEngine;

public interface IInteractable
{
    public bool IsInteractBlocked { get; }
    public bool IsInteracted { get; }
    public Vector3 Position { get; }
    public float Offset { get; }
    public GameObject GameObject { get; }
    public int ClickSort { get; }
    
    public bool IsViewed { get; }
    
    public void PreInteract();
    public void Interact();
    public void DeclineInteract();
    public void FinishInteract();
    public void BlockInteract();
    public void UnblockInteract();
    
    public void ChangeActionProvider(ListedUnityEvent actions);
}