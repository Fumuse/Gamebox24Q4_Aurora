using UnityEngine;

public class ItemInfoHandler : MonoBehaviour
{
    [SerializeField] private ItemInfoView _itemInfoView;

    public void ShowItem(ItemInfo item)
    {
        _itemInfoView.ShowDescription(item);
    }
}
