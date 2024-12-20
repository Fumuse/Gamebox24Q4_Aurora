using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Два вида тени:
/// <para>
/// 1. появляется в рандомном месте на сцене. либо исцезат практически сразу как повилась, 
/// либо будет на сцене, пока не наведем мышкой либо фонариком
/// </para>
/// 2. повляются сразу после диалогов (метод: SpawnBehindTheBack()) исчезает если персонаж отошел от тени, либо навел мышкой/фонариком
/// <para>
/// все тени безобидные, на персонажа не нападают.
/// </para>
/// </summary>
public class Shadow : Screamer
{
    [SerializeField] private int timeoutBeforeSpawn = 1500;
    [SerializeField] private bool showShadowInStart;
    [SerializeField] private float defaultAlpha = 0.85f;
    [SerializeField] private Vector2 spawnOffset = Vector2.zero;

    [Header("Расположение скримера при спавне")] 
    [SerializeField] private float xPositionMin = -1.75f;
    [SerializeField] private float xPositionMax = -6.57f;
    [SerializeField] private float yPositionMin = -2.5f;
    [SerializeField] private float yPositionMax = -3f;

    private Camera _camera;
    
    private bool _isIlluminated;
    private bool _isSpawnBehindTheBack;
    private float _distanceHideShadow = 3.5f;
    private const float YOffsetPositionShadow = -1.5f;

    private CancellationTokenSource _cts = new();

    private Vector3 CameraPosition => _camera.transform.position;

    private TeleportProvider _teleportProvider;

    protected override void Awake()
    {
        base.Awake();
        
        _camera ??= Camera.main;
    }

    private void Start()
    {
        _teleportProvider ??= GameProvidersManager.Instance.TeleportProvider;
    }

    private void OnEnable()
    {
        _cts ??= new();
        
        int alpha = 0;
        _screamerView.SetNewColorAlpha(alpha);

        Flashlight.OnFlashlightFindScreamer += OnFlashlightFindScreamer;
        
        if (_teleportProvider == null)
            _teleportProvider = GameProvidersManager.Instance.TeleportProvider;

        _teleportProvider.OnPlayerTeleported += OnPlayerTeleported;
    }

    private void OnDisable()
    {
        _cts?.Cancel();
        
        Flashlight.OnFlashlightFindScreamer -= OnFlashlightFindScreamer;
        _teleportProvider.OnPlayerTeleported -= OnPlayerTeleported;
    }

    private async void Update()
    {
        if (_isIlluminated) return;
    
        if (_isSpawnBehindTheBack == false)
        {
            FlipX();
        }
        else
        {
            if (Distance > _distanceHideShadow)
            {
                _isIlluminated = true;
                await Hide().AttachExternalCancellation(_cts.Token).SuppressCancellationThrow();
            }
        }
    }

    private async void OnFlashlightFindScreamer(Screamer screamer)
    {
        if (!screamer.Equals(this)) return;
        if (_isIlluminated) return;
        
        _isIlluminated = true;
        await Hide().AttachExternalCancellation(_cts.Token).SuppressCancellationThrow();
    }

    private async void OnMouseEnter()
    {
        if (_isIlluminated) return;
        if (showShadowInStart) return;

        _isIlluminated = true;
        await Hide().AttachExternalCancellation(_cts.Token).SuppressCancellationThrow();
    }

    public void SpawnBehindTheBack()
    {
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
        
        _isIlluminated = false;
        _isSpawnBehindTheBack = true;

        Vector2 position = PlayerPosition;
        position.y = YOffsetPositionShadow;
        position.x = PlayerPosition.x + (Player.LookDirection * spawnOffset).x;
        
        transform.position = position;

        FlipX();
        
        _screamerView.SetNewColorAlpha(defaultAlpha);
    }

    public async void Spawn()
    {
        _cts?.Cancel();
        _cts = new();

        _camera ??= Camera.main;

        _isIlluminated = false;
        _isSpawnBehindTheBack = false;

        bool isCanceled = await UniTask.Delay(timeoutBeforeSpawn, cancellationToken: _cts.Token).SuppressCancellationThrow();
        if (isCanceled) return;
        transform.position = RandomPosition();
        
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        await Show().AttachExternalCancellation(_cts.Token).SuppressCancellationThrow();
    }

    public override void Activate(bool activate)
    {
        Spawn();
    }

    private Vector3 RandomPosition() => new Vector2(
                Random.Range(CameraPosition.x + xPositionMin,
                    CameraPosition.x + xPositionMax), 
                Random.Range(yPositionMin, yPositionMax)
            );
    
    private async void OnPlayerTeleported()
    {
        await Hide().AttachExternalCancellation(_cts.Token).SuppressCancellationThrow();
    }
}