using Cysharp.Threading.Tasks;
using Foxbyte.Core;
using Foxbyte.Core.Services.AuthService;
using Foxbyte.Core.Services.ConfigurationService;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FHH.Logic
{
    public sealed class AppVersionChecker
    {
        private readonly HttpClientWithRetry _httpClient = new();
        private readonly ConfigurationService _configurationService = ServiceLocator.GetService<ConfigurationService>();

        /// <summary>
        /// Checks if a newer version than Application.version is available.
        /// </summary>
        public async UniTask<VersionCheckResult> CheckForUpdateAsync()
        {
            return await CheckForUpdateAsync(Application.version);
        }

        /// <summary>
        /// Checks if a newer version than the given currentVersion is available.
        /// currentVersion must be in "major.minor.patch" format (e.g. "0.5.1").
        /// </summary>
        public async UniTask<VersionCheckResult> CheckForUpdateAsync(string currentVersion)
        {
            if (string.IsNullOrWhiteSpace(currentVersion))
            {
                ULog.Warning("Current version is null or empty.");
                return VersionCheckResult.Failed();
            }

            Version parsedCurrentVersion;
            if (!TryParseSemanticVersion(currentVersion, out parsedCurrentVersion))
            {
                ULog.Warning($"Failed to parse current version '{currentVersion}'.");
                return VersionCheckResult.Failed();
            }

            var downloadUrl = await GetDownloadLinkFromServerAsync();
            if (string.IsNullOrWhiteSpace(downloadUrl))
            {
                return VersionCheckResult.Failed(parsedCurrentVersion);
            }

            Version remoteVersion;
            if (!TryExtractVersionFromDownloadUrl(downloadUrl, out remoteVersion))
            {
                ULog.Warning($"Failed to extract remote version from download URL '{downloadUrl}'.");
                return VersionCheckResult.Failed(parsedCurrentVersion);
            }

            var isNewer = remoteVersion > parsedCurrentVersion;

            return new VersionCheckResult(
                isSuccess: true,
                isUpdateAvailable: isNewer,
                currentVersion: parsedCurrentVersion,
                remoteVersion: remoteVersion,
                downloadUrl: downloadUrl
            );
        }

        private async UniTask<string> GetDownloadLinkFromServerAsync()
        {
            var baseUrl = _configurationService.AppSettings.ServerConfig.ApiBaseUrl;
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                ULog.Warning("ApiBaseUrl is not configured.");
                return await UniTask.FromResult<string>(null);
            }

            var endpoint = ServiceLocator.GetService<ConfigurationService>().AppSettings.ServerConfig
                .EndpointPath("AppVersion");
            var url = $"{baseUrl.TrimEnd('/')}{endpoint}";
            var headers = new Dictionary<string, string>
            {
                ["Accept"] = "text/plain, application/json"
            };

            try
            {
                var httpResponse = await _httpClient.GetAsync(url, headers);
                if (!httpResponse.IsSuccess)
                {
                    ULog.Warning(
                        $"GET {endpoint} failed: {httpResponse.StatusCode} - {httpResponse.ErrorMessage}");
                    return await UniTask.FromResult<string>(null);
                }

                var data = httpResponse.Data;
                if (string.IsNullOrWhiteSpace(data))
                {
                    ULog.Warning("Update endpoint returned empty response body.");
                    return await UniTask.FromResult<string>(null);
                }

                // In case the server wraps the URL in quotes as JSON string, strip them.
                var trimmed = data.Trim();
                if (trimmed.StartsWith("\"", StringComparison.Ordinal) &&
                    trimmed.EndsWith("\"", StringComparison.Ordinal))
                {
                    trimmed = trimmed.Substring(1, trimmed.Length - 2);
                }

                return await UniTask.FromResult(trimmed);
            }
            catch (Exception e)
            {
                ULog.Warning($"HTTP error for '{endpoint}': {e.Message}");
                return await UniTask.FromResult<string>(null);
            }
        }

        /// <summary>
        /// Extracts a version from URLs like ".../0_5_1.zip" or "...-0_5_1.zip"
        /// and converts it to a System.Version using dots ("0.5.1").
        /// </summary>
        private bool TryExtractVersionFromDownloadUrl(string downloadUrl, out Version version)
        {
            version = null;

            try
            {
                var uri = new Uri(downloadUrl);
                var lastSegment = uri.Segments[uri.Segments.Length - 1]; // e.g. "someThing2 Think_about-0_5_1.zip"
                lastSegment = lastSegment.TrimEnd('/');

                var match = Regex.Match(
                    lastSegment,
                    @"(?<version>\d+_\d+_\d+)(?=\.zip$)",
                    RegexOptions.CultureInvariant | RegexOptions.IgnoreCase
                );

                if (!match.Success)
                {
                    ULog.Warning($"No semantic version pattern found in filename '{lastSegment}'.");
                    return false;
                }

                var versionStringWithUnderscores = match.Groups["version"].Value; // e.g. "0_5_1"
                var dotVersion = versionStringWithUnderscores.Replace('_', '.'); // "0.5.1"

                return TryParseSemanticVersion(dotVersion, out version);
            }
            catch (Exception ex)
            {
                ULog.Warning($"Error while parsing version from download URL '{downloadUrl}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Parses standard "major.minor.patch" into System.Version.
        /// </summary>
        private bool TryParseSemanticVersion(string semanticVersion, out Version version)
        {
            version = null;

            if (string.IsNullOrWhiteSpace(semanticVersion))
            {
                return false;
            }

            try
            {
                // System.Version compares major > minor > build as integers
                version = new Version(semanticVersion);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Result object for a version check.
    /// </summary>
    public readonly struct VersionCheckResult
    {
        public bool IsSuccess { get; }
        public bool IsUpdateAvailable { get; }
        public Version CurrentVersion { get; }
        public Version RemoteVersion { get; }
        public string DownloadUrl { get; }

        public VersionCheckResult(
            bool isSuccess,
            bool isUpdateAvailable,
            Version currentVersion,
            Version remoteVersion,
            string downloadUrl
        )
        {
            IsSuccess = isSuccess;
            IsUpdateAvailable = isUpdateAvailable;
            CurrentVersion = currentVersion;
            RemoteVersion = remoteVersion;
            DownloadUrl = downloadUrl;
        }

        public static VersionCheckResult Failed(Version currentVersion = null)
        {
            return new VersionCheckResult(
                isSuccess: false,
                isUpdateAvailable: false,
                currentVersion: currentVersion,
                remoteVersion: null,
                downloadUrl: null
            );
        }
    }
}