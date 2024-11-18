using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private SpriteMask mask;
    [SerializeField] private Camera mainCamera;
    [SerializeField, HideInInspector] private InputReader reader;
    
    public delegate void FlashlightFindObject(IInteractable interactable);
    public static event FlashlightFindObject OnFlashlightFindObject;
    
    public delegate void FlashlightFindScreamer(Screamer screamer);
    public static event FlashlightFindScreamer OnFlashlightFindScreamer;
    
    public static Action OnFlashLightTurnOn;
    public static bool flashlightActive = false;

    private CancellationTokenSource _cts;
    private GameManager _manager;

    private void OnValidate()
    {
        reader ??= FindFirstObjectByType<InputReader>();
        mask ??= GetComponentInChildren<SpriteMask>();
        mainCamera ??= Camera.main;
    }

    private void OnEnable()
    {
        _cts = new();
        mask.gameObject.SetActive(false);
        
        InputReader.OnRightMouseClicked += OnRightMouseClicked;
    }

    private void OnDisable()
    {
        _cts?.Cancel();
        
        InputReader.OnRightMouseClicked -= OnRightMouseClicked;
    }

    private void Start()
    {
        _manager = GameManager.Instance;
    }

    private void OnRightMouseClicked(Vector2 mousePosition)
    {
        bool maskActive = mask.gameObject.activeInHierarchy;
        flashlightActive = !maskActive;
        if (maskActive)
        {
            mask.gameObject.SetActive(false);
        }
        else
        {
            TurnOnLight();
        }
    }

    private void TurnOnLight()
    {
        if (GameManager.Instance.CurrentStage == HouseStageEnum.Light) return;
        mask.gameObject.SetActive(true);
        OnFlashLightTurnOn?.Invoke();

        _manager.AcceptanceScale.SpentAcceptance(
            _manager.Settings.AcceptanceFlashlightCost
        );
        FollowMouse();
    }

    private async void FollowMouse()
    {
        while (flashlightActive)
        {
            MoveLight(reader.MousePosition);
            bool isCanceled = await UniTask.WaitForEndOfFrame(this, _cts.Token)
                .SuppressCancellationThrow();
            if (isCanceled) return;
        }
    }

    private void MoveLight(Vector2 mousePosition)
    {
        Vector3 position = mainCamera.ScreenToWorldPoint(mousePosition);
        position.z = 0;
        transform.position = position;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out IIlluminated illuminated))
        {
            bool isIlluminated = illuminated.Illuminate();
            if (isIlluminated)
            {
                if (col.TryGetComponent(out IInteractable interactable))
                {
                    OnFlashlightFindObject?.Invoke(interactable);
                }
            }
        }
        else if (col.TryGetComponent(out Screamer screamer))
        {
            OnFlashlightFindScreamer?.Invoke(screamer);
        }
    }
}