using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class LocalizedSpriteChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private LocalizeSpriteEvent localizeSpriteEvent;
    [SerializeField] private LocalizedSprite sprite;
    [SerializeField] private LocalizedSprite highlightSprite;
    [SerializeField] private LocalizedSprite pressedSprite;
    [SerializeField] private LocalizedSprite disabledSprite;
    [SerializeField] private Image targetImage;

    private Button _button;
    
    private void Awake()
    {
        _button ??= GetComponent<Button>();
    }

    private void OnValidate()
    {
        targetImage ??= GetComponent<Image>();
        localizeSpriteEvent ??= GetComponent<LocalizeSpriteEvent>();
    }

    private void OnEnable()
    {
        ChangeSprite(sprite);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ChangeSprite(highlightSprite);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ChangeSprite(sprite);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ChangeSprite(pressedSprite);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ChangeSprite(highlightSprite);
    }

    public void ChangeSprite(LocalizedSprite localizedSprite)
    {
        if (_button != null && disabledSprite != null)
        {
            if (!_button.interactable)
            {
                localizeSpriteEvent.AssetReference = disabledSprite;
                return;
            }
        }

        if (localizedSprite == null) return;
        localizeSpriteEvent.AssetReference = localizedSprite;
    }
}
