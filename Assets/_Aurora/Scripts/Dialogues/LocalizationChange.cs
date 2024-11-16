using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizationChange : MonoBehaviour
{
    private bool _lang;

    public void ChangeLanguage()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[(_lang=!_lang)?0:1];
    }
}
