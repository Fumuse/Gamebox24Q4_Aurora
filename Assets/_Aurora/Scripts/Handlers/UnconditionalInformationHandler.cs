using System.Linq;
using UnityEngine;

public class UnconditionalInformationHandler : MonoBehaviour
{
    [SerializeField] private ActionSettingsKeyPair[] settingsPair;
    
    private WhisperProvider _whisperProvider;

    public void Init()
    {
        _whisperProvider = GameProvidersManager.Instance.WhisperProvider;
    }
    
    private void OnEnable()
    {
        DialogueProvider.OnUnconditionalInformation += OnUnconditionalInformationDialogue;
    }

    private void OnDisable()
    {
        DialogueProvider.OnUnconditionalInformation -= OnUnconditionalInformationDialogue;
    }

    private void OnUnconditionalInformationDialogue(string dialogueEndId)
    {
        ActionSettingsKeyPair pair = settingsPair.FirstOrDefault((item) => item.key == dialogueEndId);
        if (pair == null) return;
        
        _whisperProvider.EmptyExecute(pair.actionSetting);
    }
}