public class GrandmaKitchenState : TutorialBaseState
{
    private IInteractable _grandma;
    private TeleportProvider _teleportProvider;
    
    public GrandmaKitchenState(TutorialStateMachine stateMachine) : base(stateMachine)
    {}

    public override void Enter()
    {
        _grandma = GetInteractableByKey("Room_4_Grandma");
        if (_grandma == null) return;
        
        _teleportProvider = GameProvidersManager.Instance.TeleportProvider;
        
        InteractableObject.OnCancelInteract += OnCancelInteract;
    }

    public override void Tick()
    {}

    public override void Exit()
    {
        Door kitchenDoor = GetDoorByKey("Room_4_DoorRight");
        kitchenDoor.ReturnRealConnectedDoor();

        _teleportProvider.OnPlayerTeleported -= OnPlayerTeleported;
    }

    private void OnCancelInteract(IInteractable interactable)
    {
        if (!interactable.Equals(_grandma)) return;
        
        InteractableObject.BlockInteractedObject(_grandma);
        InteractableObject.OnCancelInteract -= OnCancelInteract;
        
        //Изменяем направление двери Room_4_DoorLeft до Loc_1
        ChangeDoorConnectedDoor("Room_4_DoorRight", "Room_1_DoorLeft");

        //Выключаем дверцу
        LockDoor("Room_1_DoorLeft");
        //Включаем дверцу в Loc_1
        UnlockDoor("Room_4_DoorRight");
        UnlockDoor("Room_1_DoorDown");
        _teleportProvider.OnPlayerTeleported += OnPlayerTeleported;
    }

    private void OnPlayerTeleported()
    {
        InteractableObject.UnblockInteractedObject(_grandma);
        stateMachine.SwitchState(new CellarState(stateMachine));
    }
}