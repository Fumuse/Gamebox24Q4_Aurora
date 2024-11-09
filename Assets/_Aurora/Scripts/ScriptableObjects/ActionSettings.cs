using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "ActionSettings", menuName = "Actions/ActionSettings")]
public class ActionSettings : ScriptableObject
{
    [SerializeField] private string whisperTextKey;
    public string WhisperTextKey => whisperTextKey;

    //TODO: поменять на правильный объект
    [SerializeField] private ScriptableObject dialogueRoot;
    public ScriptableObject DialogueRoot => dialogueRoot;

    [SerializeField] private string catSceneClipKey;
    public string CatSceneClipKey => catSceneClipKey;

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
}