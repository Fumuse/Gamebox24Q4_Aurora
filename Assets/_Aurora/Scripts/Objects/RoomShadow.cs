using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RoomShadow : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private GameManager _manager;
    private Timer _timer;
    private float _overlayStep;
    private float _maxOverlay;
    
    private void OnValidate()
    {
        spriteRenderer ??= GetComponent<SpriteRenderer>();
    }

    public void Init()
    {
        _manager = GameManager.Instance;
        _timer = _manager.Timer;

        _maxOverlay = _manager.Settings.MaxRoomShadowOverlayOpacity / 255f;
        _overlayStep = _manager.Settings.StepRoomShadowOverlayOpacity / 255f;
    }

    private void OnEnable()
    {
        Timer.OnAfterFlashlightTimeChanged += OnAfterFlashlightTimeChanged;
    }

    private void OnDisable()
    {
        Timer.OnAfterFlashlightTimeChanged -= OnAfterFlashlightTimeChanged;
    }

    public void Change(HouseStageEnum stage)
    {
        spriteRenderer.enabled = stage != HouseStageEnum.Light;
        
        if (stage != HouseStageEnum.Light)
        {
            spriteRenderer.DOColor(GetShadowColor(_timer.TimePassedAfterFlashlight), 3f);
        }
    }

    private Color GetShadowColor(int timeWithoutLight = 0)
    {
        Color shadowColor = spriteRenderer.color;
        if (timeWithoutLight == 0 || GameManager.Instance.TutorialStage) 
            shadowColor.a = _maxOverlay;
        else
        {
            int stepsCount = Mathf.FloorToInt(timeWithoutLight / 10);
            float currentOverlay = _maxOverlay - (stepsCount * _overlayStep);
            if (currentOverlay < 0) currentOverlay = 0;
            shadowColor.a = currentOverlay;
        }

        return shadowColor;
    }

    private void OnAfterFlashlightTimeChanged()
    {
        Change(_manager.CurrentStage);
    }
}