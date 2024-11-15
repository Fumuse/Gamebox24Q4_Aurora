using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseHoverDetector : MonoBehaviour
{
    [SerializeField] private LayerMask interactableObjectMask;
    
    private CancellationTokenSource _cts = new();
    private Camera _camera;

    private List<GameObject> _listsOfObjects = new();
    private Dictionary<string, InteractableObjectUI> _listsOfObjectsUI = new();

    private List<InteractableObjectUI> _activeInLastIterate = new();
    
    public void Init()
    {
        _camera = Camera.main;
        Detector();
    }

    private void OnEnable()
    {
        if (_cts == null)
        {
            _cts = new();
        }
    }

    private void OnDisable()
    {
        _cts?.Cancel();
    }

    private async void Detector()
    {
        while (true)
        {
            Vector2 mousePosition = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero, interactableObjectMask);

            List<InteractableObjectUI> activeInCurrentIterate = new();
            if (hits.Length > 0)
            {
                foreach (RaycastHit2D hit in hits)
                {
                    if (_listsOfObjects.Contains(hit.transform.gameObject))
                    {
                        if (_listsOfObjectsUI.TryGetValue(hit.transform.name, out InteractableObjectUI ui)) {
                            ui.OnRayMouseEnter();
                            activeInCurrentIterate.Add(ui);
                        }
                    }
                    else
                    {
                        _listsOfObjects.Add(hit.transform.gameObject);
                        if (hit.transform.TryGetComponent(out InteractableObjectUI ui))
                        {
                            _listsOfObjectsUI.Add(hit.transform.name, ui);
                            ui.OnRayMouseEnter();
                            activeInCurrentIterate.Add(ui);
                        }
                    }
                }
            }

            foreach (var ui in _activeInLastIterate)
            {
                if (!activeInCurrentIterate.Contains(ui))
                {
                    ui.OnRayMouseExit();
                }
            }

            _activeInLastIterate = activeInCurrentIterate;
            
            bool isCanceled = await UniTask.WaitForEndOfFrame(this, cancellationToken: _cts.Token)
                .SuppressCancellationThrow();
            if (isCanceled) return;
        }
    }
}