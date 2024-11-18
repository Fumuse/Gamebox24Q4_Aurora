using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "ActionSettings", menuName = "Actions/ActionSettings")]
public class ActionSettings : ScriptableObject
{
    [SerializeField] private LocalizedString whisperTextKey;
    public LocalizedString WhisperText => whisperTextKey;

    [SerializeField] private DialogueNode dialogueRoot;
    public DialogueNode DialogueRoot => dialogueRoot;

    [SerializeField] private ItemInfo item;
    public ItemInfo Item => item;

    [SerializeField] private LocalizedAsset<VideoClip> catSceneClip;
    public LocalizedAsset<VideoClip> CatSceneClip => catSceneClip;

    [SerializeField, Tooltip("Сколько времени расходуется за выполнение действия")] 
    private int timeCost = 0;
    public int TimeCost => timeCost;

    [SerializeField, Tooltip("Сколько принятия расходуется за выполнение действия")]
    private int acceptanceCost = 0;
    public int AcceptanceCost => acceptanceCost;

    [SerializeField] private ActionEvent changeObjectEventAfterPlay;
    public ActionEvent ChangeObjectEventAfterPlay => changeObjectEventAfterPlay;
    
    [SerializeField] private ActionSettings changeObjectSettingsAfterPlay;
    public ActionSettings ChangeActionSettingsAfterPlay => changeObjectSettingsAfterPlay;

    [SerializeField] private Tag[] tagsToAddAfterAction;
    public Tag[] TagsToAddAfterAction => tagsToAddAfterAction;

    [SerializeField] private ScreamerEnum screamerType;
    public ScreamerEnum ScreamerType => screamerType;
    [SerializeField] private Vector3 screamerSpawnPosition;
    public Vector3 ScreamerSpawnPosition => screamerSpawnPosition;
}