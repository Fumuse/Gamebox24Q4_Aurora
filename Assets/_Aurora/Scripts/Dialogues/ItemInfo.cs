using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName = "Item/ItemInfo")]
public class ItemInfo : ScriptableObject
{
    [SerializeField] private LocalizedString _nameItem;
    [SerializeField] private Sprite _itemSprite;
    [SerializeField] private LocalizedString _description;

    public LocalizedString NameItem => _nameItem;
    
    public LocalizedString Description => _description;

    public Sprite ItemSprite => _itemSprite;
}
