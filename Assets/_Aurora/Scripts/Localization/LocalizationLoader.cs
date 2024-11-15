using System.Threading;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocalizationLoader : Singleton<LocalizationLoader>
{
    private const string PreferredLocaleKey = "selected-locale";
    
    private bool _isSetLocaleActive = false;
    private CancellationTokenSource _cts = new();

    private void OnEnable()
    {
        _cts = new();
    }

    private void OnDisable()
    {
        _cts?.Cancel();
    }

    public void ChangeLocale(string localeCode)
    {
        if (_isSetLocaleActive) return;
        Locale locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
        
        SetLocale(locale);
    }

    private void SetLocale(Locale locale)
    {
        _isSetLocaleActive = true;
        
        LocalizationSettings.SelectedLocale = locale;
        SavePreferredLocale(locale);

        _isSetLocaleActive = false;
    }

    private void SavePreferredLocale(Locale locale)
    {
        PlayerPrefs.SetString(PreferredLocaleKey, locale.Identifier.Code);
        PlayerPrefs.Save();
    }
}
