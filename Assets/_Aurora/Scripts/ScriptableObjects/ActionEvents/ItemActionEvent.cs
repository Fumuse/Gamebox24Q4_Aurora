using UnityEngine;

[CreateAssetMenu(fileName = "ItemActionEvent", menuName = "Actions/ActionEvents/ItemActionEvent")]
public class ItemActionEvent : ActionEvent
{
    private ItemInfoProvider _itemProvider;

    private void Init()
    {
        if (_itemProvider == null)
        {
            _itemProvider = GameProvidersManager.Instance.ItemInfoProvider;
        }
    }

    public override ListedUnityEvent GetEvent(ActionSettings settings)
    {
        Init();

        ListedUnityEvent unityEvent = new ListedUnityEvent();
        unityEvent.AddListener(() => _itemProvider.Execute(settings));
        return unityEvent;
    }
}
