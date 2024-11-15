using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Два вида тени:
/// <para>
/// 1. появляется в рандомном месте на сцене. либо исцезат практически сразу как повиласьь, 
/// либо будет на сцене, пока не навидем мышкой либо фонариком
/// </para>
/// 2. повляются сразу после диалогов (метод: SpawnBehindTheBack()) исчезает если персонаж отошел от тени, либо навел мышкой/фонариком
/// <para>
/// все тени безодиные, на персонажа не нападают.
/// </para>para>
/// </summary>

public class Shadow : Screamer
{
    [SerializeField] private float _timeLife;
    [SerializeField] private int _timeoutBeforeSpawn = 1500;
    [SerializeField] private bool _showShadowInStart;
    [SerializeField] private float _defaultAlpha = 0.85f;

    private bool _isIlluminated;
    private bool _isSpawnBehindTheBack;
    private float _distanceHideShadow = 2.5f;
    private const float _xPositionMin = -1.75f;
    private const float _xPositionMax = 6.57f;
    private const float _yPositionMin = -0.76f;
    private const float _yPositionMax = -1.73f;
    private const float _yOffsetPositionShadow = -1.5f;

    private void Start()
    {
        int alpha = 0;

        if (_showShadowInStart)
        {
            _isIlluminated = false;
            _isSpawnBehindTheBack = false;

            alpha = 1;    
        }
          
        _screamerView.SetNewColorAlpha(alpha);
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

    private async void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isIlluminated) return;
        
        if (collision.TryGetComponent(out Flashlight flashlight))
        {
            _isIlluminated = true;
            UniTask task = Hide();
            await task;
        }
    }

    private async void OnMouseEnter()
    {
        if (_isIlluminated) return;

        if (_showShadowInStart) return;
        
        _isIlluminated = true;
        UniTask task = Hide();
        await task;
    }

    public void SpawnBehindTheBack()
    {
        _isIlluminated = false;
        _isSpawnBehindTheBack = true;

        Vector2 position = PlayerPosition;
        position.y = _yOffsetPositionShadow;
        transform.position = position;

        FlipX();
       _screamerView.SetNewColorAlpha(_defaultAlpha);
    }

    public async void Spawn()
    {
        _isIlluminated = false;
        _isSpawnBehindTheBack = false;

        await UniTask.Delay(_timeoutBeforeSpawn);
        transform.position = RandomPosition();

        await Show();

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

    private Vector3 RandomPosition() =>new Vector2(Random.Range(Camera.main.transform.position.x + _xPositionMin, Camera.main.transform.position.x+_xPositionMax), Random.Range( _yPositionMin, _yPositionMax));
}
