using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : Screamer
{
    [SerializeField] private float _timeLife;

    private bool _isIlluminated;

    private const float _xPositionMin = -3.17f;
    private const float _xPositionMax = 2.64f;
    private const float _yPositionMin = -1.4f;
    private const float _yPositionMax = -2.41f;

    public async void Spawn()
    {
        _isIlluminated = false;

        await UniTask.Delay(1500);
        transform.position = RandomPosition();

        await Show();
        
        int hide = Random.Range(0, 50);//magic numbers =)

        if (hide < 20)
        {
            await Hide();
            _isIlluminated = true;
        }
    }

    public override void Activate(bool activate)
    {
        Spawn();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isIlluminated) return;

        if (collision.TryGetComponent(out Flashlight flashlight))
        {
            _isIlluminated = true;
            UniTask task = Hide();
        }
    }

    private Vector3 RandomPosition() =>new Vector2(Random.Range(Camera.main.transform.position.x + _xPositionMin, Camera.main.transform.position.x+_xPositionMax), Random.Range( _yPositionMin, _yPositionMax));
}
