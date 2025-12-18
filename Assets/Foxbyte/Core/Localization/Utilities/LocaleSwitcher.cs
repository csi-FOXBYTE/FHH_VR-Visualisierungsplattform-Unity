using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Foxbyte.Core.Localization.Utilities
{
    /// <summary>
    /// Provides methods to switch the current locale.
    /// </summary>
    public class LocaleSwitcher : IAppServiceAsync
    {
        private bool _isReady;
        private List<Locale> _cachedLocales = new List<Locale>();
        
        public async UniTask InitServiceAsync()
        {
            await _init();
        }

        public async UniTask DisposeServiceAsync()
        {
            _cachedLocales.Clear();
            _isReady = false;
            await UniTask.CompletedTask;
        }

        public int NextLocaleIndex;

        public LocaleSwitcher()
        {
        }

        //public void InitService()
        //{
        //    _init().Forget();
        //}

        //public void DisposeService()
        //{
        //}

        private async UniTask _init()
        {
            await LocalizationSettings.InitializationOperation.ToUniTask();
            _cachedLocales = LocalizationSettings.AvailableLocales.Locales;
            _isReady = true;

            ////var savedLocale = await ServiceLocator.Instance.GetService<PersistenceService>().LoadDataAsync<StringWrapper>("Locale");
            //var savedLocale = await _serviceLocator.GetService<PersistenceService>().LoadDataAsync<StringWrapper>("Locale");
            //if (savedLocale != null)
            //{
            //    SwitchLocaleString(savedLocale.Value);
            //    Debug.Log("Loaded locale: " + savedLocale.Value);
            //}
            await UniTask.CompletedTask;
        }

        public async UniTask<List<string>> GetAvailableLocaleNamesAsync()
        {
            await EnsureReadyAsync();
            List<string> fullLanguageNames = new List<string>(_cachedLocales.Count);
            foreach (var locale in _cachedLocales)
            {
                CultureInfo cultureInfo = locale.Identifier.CultureInfo;
                fullLanguageNames.Add(cultureInfo.DisplayName);
            }
            return fullLanguageNames;
            /*var availableLocales = LocalizationSettings.AvailableLocales.Locales;
            await UniTask.Yield();
            List<string> fullLanguageNames = new List<string>();
            foreach (var locale in availableLocales)
            {
                // Get the CultureInfo object from the Locale
                CultureInfo cultureInfo = locale.Identifier.CultureInfo;
                // Use the DisplayName property of the CultureInfo to get the full name of the language
                string languageDisplayName = cultureInfo.DisplayName;
                // trimming the end, e.g. (en)
                //languageDisplayName = languageDisplayName.Split('(', ')')[0].Trim();
                fullLanguageNames.Add(languageDisplayName);
            }
            return fullLanguageNames;*/
        }

        public bool IsGerman()
        {
            var selected = LocalizationSettings.SelectedLocale;
            if (selected == null) return false;
            var l = selected.Identifier.CultureInfo.TwoLetterISOLanguageName;
            return l.Equals("de", StringComparison.InvariantCultureIgnoreCase);
            //var l = LocalizationSettings.SelectedLocale.Identifier.CultureInfo.TwoLetterISOLanguageName;
            //return l.Equals("de", System.StringComparison.InvariantCultureIgnoreCase);
        }

        public bool IsTwoLetterLanguage(string twoLetters)
        {
            return LocalizationSettings.SelectedLocale.Identifier.CultureInfo.TwoLetterISOLanguageName.Equals(twoLetters, System.StringComparison.InvariantCultureIgnoreCase);
        }

        public List<Locale> GetAvailableLocales()
        {
            return _cachedLocales;
        }

        public string GetCurrentLocaleName()
        {
            return LocalizationSettings.SelectedLocale?.LocaleName ?? string.Empty;
        }

        public Locale GetCurrentLocale()
        {
            return LocalizationSettings.SelectedLocale;
        }

        public string GetCurrentLocaleDisplayName()
        {
            var selected = LocalizationSettings.SelectedLocale;
            return selected != null ? selected.Identifier.CultureInfo.DisplayName : string.Empty;
        }
        
        [ContextMenu("Switch to next locale")]
        public void SwitchToNextLocale()
        {
            if (!_isReady || _cachedLocales.Count == 0) return;

            var current = LocalizationSettings.SelectedLocale;
            var currentIndex = _cachedLocales.IndexOf(current);
            var nextIndex = (currentIndex + 1) % _cachedLocales.Count;
            LocalizationSettings.SelectedLocale = _cachedLocales[nextIndex];
            //var locales = LocalizationSettings.AvailableLocales.Locales;
            //var currentLocale = LocalizationSettings.SelectedLocale;
            //var currentLocaleIndex = locales.IndexOf(currentLocale);
            //NextLocaleIndex = (currentLocaleIndex + 1) % locales.Count;
            //var nextLocale = locales[NextLocaleIndex];
            //LocalizationSettings.SelectedLocale = nextLocale;
        }

        public void SwitchLocale(string localeName)
        {
            SwitchLocaleString(localeName);
            //SaveLocale().Forget();
        }
        public void SwitchLocale(Locale locale)
        {
            SwitchLocaleType(locale);
            SaveLocale().Forget();
        }

        private void SwitchLocaleString(string localeName)
        {
            var availableLocales = LocalizationSettings.AvailableLocales.Locales;
            foreach (var locale in availableLocales)
            {
                if (locale.Identifier.CultureInfo.DisplayName.Equals(localeName, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    // Set the selected locale.
                    LocalizationSettings.SelectedLocale = locale;
                    ULog.Info("Switching language to: " + localeName);
                    break;
                }
            }
        }

        private void SwitchLocaleType(Locale locale)
        {
            var localeName = locale.Identifier.CultureInfo.DisplayName;
            SwitchLocaleString(localeName);
        }

        private async UniTask SaveLocale()
        {
            //var localeIdentifier = new StringWrapper(LocalizationSettings.SelectedLocale.Identifier.CultureInfo.DisplayName);
            ////await ServiceLocator.Instance.GetService<PersistenceService>().SaveDataAsync("Locale", localeIdentifier);
            //await _serviceLocator.GetService<PersistenceService>().SaveDataAsync("Locale", localeIdentifier);
            await UniTask.CompletedTask;
        }

        public bool IsCurrentLocale(string localeName)
        {
            var selected = LocalizationSettings.SelectedLocale;
            return selected != null && selected.LocaleName.Contains(localeName);
            //return LocalizationSettings.SelectedLocale.LocaleName.Contains(localeName);
        }

        private async UniTask EnsureReadyAsync()
        {
            if (_isReady) return;
            await LocalizationSettings.InitializationOperation.ToUniTask();
            _cachedLocales = LocalizationSettings.AvailableLocales.Locales;
            _isReady = true;
        }

        // Testing purposes
        //#if UNITY_EDITOR
        //        private void Update()
        //        {
        //            if (Input.GetKeyDown(KeyCode.L))
        //            {
        //                SwitchToNextLocale();
        //            }
        //        }
        //#endif


    }
}