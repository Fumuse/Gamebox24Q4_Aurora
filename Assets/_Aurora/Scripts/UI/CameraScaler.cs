using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraScaler : MonoBehaviour
{
    [SerializeField] private Vector2 defaultResolution = new Vector2(1920, 1080);
    [SerializeField] private float widthOrHeight = 0;

    private Camera _camera;

    private float _initialSize;
    private float _targetAspect;

    private float _initialFov;
    private float _horizontalFov = 120f;
    
    private int _oldScreenWidth;
    private int _oldScreenHeight;

    private CancellationTokenSource _cts;

    private void OnEnable()
    {
        _cts = new CancellationTokenSource();
        
        _camera ??= GetComponent<Camera>();
        _initialSize = _camera.orthographicSize;
        _targetAspect = defaultResolution.x / defaultResolution.y;
        _initialFov = _camera.fieldOfView;
        _horizontalFov = CalcVerticalFov(_initialFov, 1 / _targetAspect);

        WindowResize();
    }

    private void OnDisable()
    {
        if (_cts != null)
            _cts.Cancel();
    }

    private async void WindowResize()
    {
        while (true)
        {
            bool isCanceled = await UniTask
                .WaitUntil(
                    () => _oldScreenHeight != Screen.height || _oldScreenWidth != Screen.width, 
                        cancellationToken: _cts.Token
                    )
                .SuppressCancellationThrow();
            if (isCanceled) return;

            _oldScreenHeight = Screen.height;
            _oldScreenWidth = Screen.width;

            CameraScale();
        }
    }

    private void CameraScale()
    {
        if (_camera.orthographic)
        {
            float constantWidthSize = _initialSize * (_targetAspect / _camera.aspect);
            _camera.orthographicSize = Mathf.Lerp(constantWidthSize, _initialSize, widthOrHeight);
        }
        else
        {
            float constantWidthFov = CalcVerticalFov(_horizontalFov, _camera.aspect);
            _camera.fieldOfView = Mathf.Lerp(constantWidthFov, _initialFov, widthOrHeight);
        }
    }

    private float CalcVerticalFov(float horizontalFovInDeg, float aspectRatio)
    {
        float horizontalFovInRads = horizontalFovInDeg * Mathf.Deg2Rad;
        float verticalFovInRads = 2 * MathF.Atan(Mathf.Tan(horizontalFovInRads / 2) / aspectRatio);
        return horizontalFovInRads * verticalFovInRads;
    }
}
