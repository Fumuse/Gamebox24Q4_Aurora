using System;
using UnityEngine;

public class GameProvidersManager : PersistentSingleton<GameProvidersManager>
{
    [SerializeField] private DialogueProvider dialogueProvider;
    [SerializeField] private TeleportProvider teleportProvider;
    [SerializeField] private WhisperProvider whisperProvider;
    [SerializeField] private ShowVideoSceneProvider videoSceneProvider;

    public DialogueProvider DialogueProvider => dialogueProvider;
    public TeleportProvider TeleportProvider => teleportProvider;
    public WhisperProvider WhisperProvider => whisperProvider;
    public ShowVideoSceneProvider VideoSceneProvider => videoSceneProvider;

    private void OnValidate()
    {
        dialogueProvider ??= FindFirstObjectByType<DialogueProvider>();
        teleportProvider ??= FindFirstObjectByType<TeleportProvider>();
        whisperProvider ??= FindFirstObjectByType<WhisperProvider>();
        videoSceneProvider ??= FindFirstObjectByType<ShowVideoSceneProvider>();
    }
}