using UnityEngine;

public class GameProvidersManager : Singleton<GameProvidersManager>
{
    [SerializeField] private DialogueProvider dialogueProvider;
    [SerializeField] private ItemInfoProvider itemInfoProvider;
    [SerializeField] private TeleportProvider teleportProvider;
    [SerializeField] private WhisperProvider whisperProvider;
    [SerializeField] private ShowVideoSceneProvider videoSceneProvider;
    [SerializeField] private ScreamerProvider screamerProvider;

    public DialogueProvider DialogueProvider => dialogueProvider;
    public ItemInfoProvider ItemInfoProvider => itemInfoProvider;
    public TeleportProvider TeleportProvider => teleportProvider;
    public WhisperProvider WhisperProvider => whisperProvider;
    public ShowVideoSceneProvider VideoSceneProvider => videoSceneProvider;
    public ScreamerProvider ScreamerProvider => screamerProvider;

    private void OnValidate()
    {
        dialogueProvider ??= FindFirstObjectByType<DialogueProvider>();
        itemInfoProvider ??= FindFirstObjectByType<ItemInfoProvider>();
        teleportProvider ??= FindFirstObjectByType<TeleportProvider>();
        whisperProvider ??= FindFirstObjectByType<WhisperProvider>();
        videoSceneProvider ??= FindFirstObjectByType<ShowVideoSceneProvider>();
        screamerProvider ??= FindFirstObjectByType<ScreamerProvider>();
    }

    public void Init()
    {
        if (Instance == null)
        {
            this.Awake();
        }
    }
}