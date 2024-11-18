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

    private Camera _camera;
    
    private bool _isIlluminated;
    private bool _isSpawnBehindTheBack;
    private float _distanceHideShadow = 2.5f;
    private const float XPositionMin = -1.75f;
    private const float XPositionMax = 6.57f;
    private const float YPositionMin = -0.76f;
    private const float YPositionMax = -1.73f;
    private const float YOffsetPositionShadow = -1.5f;

    private CancellationTokenSource _cts = new();

    private Vector3 CameraPosition => _camera.transform.position;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        _cts ??= new();
        
        int alpha = 0;

        if (showShadowInStart)
        {
            _isIlluminated = false;
            _isSpawnBehindTheBack = false;

            alpha = 1;
        }

        _screamerView.SetNewColorAlpha(alpha);

        Flashlight.OnFlashlightFindScreamer += OnFlashlightFindScreamer;
    }

    private void OnDisable()
    {
        _cts?.Cancel();
        
        Flashlight.OnFlashlightFindScreamer -= OnFlashlightFindScreamer;
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
                await Hide();
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
        transform.position = position;

        FlipX();
        
        _screamerView.SetNewColorAlpha(defaultAlpha);
    }

    public async void Spawn()
    {
        _isIlluminated = false;
        _isSpawnBehindTheBack = false;

        bool isCanceled = await UniTask.Delay(timeoutBeforeSpawn, cancellationToken: _cts.Token).SuppressCancellationThrow();
        if (isCanceled) return;
        transform.position = RandomPosition();

        isCanceled = await Show().AttachExternalCancellation(_cts.Token).SuppressCancellationThrow();
        if (isCanceled) return;

        if (Random.Range(0, 50) > 20)
        {
            _isIlluminated = true;
            await Hide();
        }
    }

    public override void Activate(bool activate)
    {
        Spawn();
    }

    private Vector3 RandomPosition() => new Vector2(
                Random.Range(CameraPosition.x + XPositionMin,
                    CameraPosition.x + XPositionMax), 
                Random.Range(YPositionMin, YPositionMax)
            );
}