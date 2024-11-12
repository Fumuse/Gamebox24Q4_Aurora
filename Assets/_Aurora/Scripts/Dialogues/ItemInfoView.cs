using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class ItemInfoView : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameItemTMP;
    [SerializeField] private TMP_Text _descriptionTMP;
    [SerializeField] private Image _itemSprite;
    [SerializeField] private Canvas _itemCanvas;

    private string _nameItem;
    private string _descriptionItem;

    private ItemInfo _item;

    private void OnEnableView()
    {
        _item.NameItem.StringChanged += OnNameStringChange;
        _item.Description.StringChanged+= OnDescriptionStringChange;
    }

    private void OnDisableView()
    {
        _item.NameItem.StringChanged -= OnNameStringChange;
        _item.Description.StringChanged -= OnDescriptionStringChange;
    }

    private void OnNameStringChange(string value)=>_nameItem = value;
    private void OnDescriptionStringChange(string value)=>_descriptionItem = value;

    public void Show()
    {
        _itemCanvas.enabled = true;
        OnEnableView();
    }

    public void Hide()
    {
        OnDisableView();
        _itemCanvas.enabled = false;
    }
    
    public void ShowDescription(ItemInfo item)
    {
        _item = item;

        Show();
        ShowNameItem(_nameItem);
        ShowDescription(_descriptionItem);
        ShowSpriteIten(_item.ItemSprite);
    }
    
    private void ShowNameItem(string name)=>_nameItemTMP.text = name;

    private void ShowDescription(string description) => _descriptionTMP.text = description;

    private void ShowSpriteIten(Sprite itemSprite)=>_itemSprite.sprite = itemSprite;
}
