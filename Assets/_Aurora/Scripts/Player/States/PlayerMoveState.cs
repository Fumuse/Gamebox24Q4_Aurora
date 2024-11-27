using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    private LayerMask _interactableObjectMask;
    
    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine)
    {}

    public override void Enter()
    {
        InputReader.OnMouseClicked += OnMouseClicked;

        _interactableObjectMask = GameManager.Instance.InteractableObjectLayerMask;
        
        SetMovingAnimate();
    }

    public override void Tick()
    {
        CheckClickToUI();
        Rotate();
        Move();
        
        MoveAnimation();
    }

    public override void Exit()
    {
        InputReader.OnMouseClicked -= OnMouseClicked;
    }

    private void OnMouseClicked(Vector2 mousePosition)
    {
        if (isClickedToUI) return;

        targetPosition = stateMachine.MainCamera.ScreenToWorldPoint(mousePosition);
        Collider2D[] clickedColliders = Physics2D.OverlapPointAll(targetPosition, _interactableObjectMask);

        List<IInteractable> clickedItems = new();
        if (clickedColliders is {Length: > 0})
        {
            foreach (Collider2D clickedCollider in clickedColliders)
            {
                if (clickedCollider.TryGetComponent(out IInteractable interactable))
                {
                    if (interactable.IsInteractBlocked) continue;
                    if (!interactable.IsViewed) continue;

                    if (Slenderman.IsActive)
                    {
                        if (!clickedCollider.TryGetComponent(out Door door))
                        {
                            continue;
                        }
                    }
                    
                    clickedItems.Add(interactable);
                }
            }
        }

        if (clickedItems.Count > 0)
        {
            if (clickedItems.Count > 1)
            {
                clickedItems.Sort((a, b) => b.ClickSort.CompareTo(a.ClickSort));
            }
            
            stateMachine.SwitchState(new PlayerInteractState(stateMachine, clickedItems.FirstOrDefault()));
            return;
        }

        IsMoving = true; 
    }
}