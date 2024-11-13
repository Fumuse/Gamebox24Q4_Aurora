﻿using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private SpriteMask mask;
    [SerializeField] private Camera mainCamera;
    [SerializeField, HideInInspector] private InputReader reader;
    
    public static Action OnFlashLightTurnOn;
    public static bool flashlightActive = false;

    private CancellationTokenSource _cts;

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
}