using Cysharp.Threading.Tasks;
using FHH.Logic.Models;
using Foxbyte.Core;
using Foxbyte.Presentation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using FHH.Logic;
using Foxbyte.Core.Services.AuthService;
using Foxbyte.Core.Services.ConfigurationService;

namespace FHH.UI.Startingpoints
{
    public class StartingpointsModel : PresenterModelBase
    {
        private List<StartingPoint> _startingPoints = new List<StartingPoint>();

        public Project CurrentProject { get; private set; }
        public IReadOnlyList<StartingPoint> StartingPoints => _startingPoints;

        public void SetProject(Project project)
        {
            CurrentProject = project ?? throw new ArgumentNullException(nameof(project));

            //_startingPoints.Clear();
            if (CurrentProject.StartingPoints != null)
            {
                _startingPoints.AddRange(CurrentProject.StartingPoints);
                SortStartingPoints();
            }
        }

        public override async UniTask InitializeWithDataAsync<T>(T data)
        {
            _startingPoints.Clear();
            await UniTask.CompletedTask;
            //if (data is List<StartingPoint> startingPoints)
            //{
            //    _startingPoints.Clear();
            //    _startingPoints.AddRange(startingPoints);
            //    SortStartingPoints();
            //}
        }

        public override async UniTask LoadDataAsync()
        {
            await UniTask.CompletedTask;
        }

        internal async UniTask GetPublicStartingPointsAsync()
        {
            try
            {
                var endpoint = ServiceLocator.GetService<ConfigurationService>().AppSettings.ServerConfig
                    .EndpointPath("PublicStartingPoints");
                var publicStartingPoints = await GetDataFromPublicEndpointAsync<List<StartingPoint>>(endpoint);
                
                for (int i = 0; i < (publicStartingPoints?.Count ?? 0); i++)
                {
                    ULog.Info($"Public Starting Point {i}: ID={publicStartingPoints[i].Id}, Name={publicStartingPoints[i].Name}");
                }
                if (publicStartingPoints != null)
                {
                    _startingPoints.AddRange(publicStartingPoints);
                    SortStartingPoints();
                }
            }
            catch (Exception ex)
            {
                ULog.Warning($"Failed to retrieve public starting points: {ex.Message}");
                return;
            }
        }

        private async UniTask<T> GetDataFromPublicEndpointAsync<T>(string endpoint)
        {
            var baseUrl = ServiceLocator.GetService<ConfigurationService>().AppSettings.ServerConfig.ApiBaseUrl;
            var http = new HttpClientWithRetry();
            var url = $"{baseUrl?.TrimEnd('/')}{endpoint}";
            var headers = new Dictionary<string, string> { ["Accept"] = "application/json" };
            try
            {
                var httpResponse = await http.GetAsync(url, headers);
                if (!httpResponse.IsSuccess)
                {
                    ULog.Warning($"GET {endpoint} failed: {httpResponse.StatusCode} - {httpResponse.ErrorMessage}");
                    return await UniTask.FromResult(default(T));
                }
                return JsonConvert.DeserializeObject<T>(httpResponse.Data);
            }
            catch (Exception e)
            {
                ULog.Warning($"HTTP error for '{endpoint}': {e.Message}");
                return await UniTask.FromResult(default(T));
            }
        }

        private void SortStartingPoints()
        {
            _startingPoints.Sort((a, b) =>
            {
                if (a == null && b == null)
                {
                    return 0;
                }

                if (a == null)
                {
                    return 1;
                }

                if (b == null)
                {
                    return -1;
                }

                var aName = string.IsNullOrWhiteSpace(a.Name) ? string.Empty : a.Name;
                var bName = string.IsNullOrWhiteSpace(b.Name) ? string.Empty : b.Name;

                var nameCompare = NaturalStringCompare(aName, bName);
                if (nameCompare != 0)
                {
                    return nameCompare;
                }

                // Optional: stable tiebreaker if names are identical
                return string.Compare(a.Id, b.Id, StringComparison.OrdinalIgnoreCase);
            });
        }

        /// <summary>
        /// Compares two strings using "natural" ordering:
        /// digit sequences are compared as numbers, everything else lexicographically.
        /// This is schema-agnostic and works for arbitrary names like
        /// "Starting Point 1", "X2_A", "Build10", etc.
        /// </summary>
        private int NaturalStringCompare(string x, string y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (x == null)
            {
                return 1;
            }

            if (y == null)
            {
                return -1;
            }

            var i = 0;
            var j = 0;
            var xLen = x.Length;
            var yLen = y.Length;

            while (i < xLen && j < yLen)
            {
                var cx = x[i];
                var cy = y[j];

                var xIsDigit = char.IsDigit(cx);
                var yIsDigit = char.IsDigit(cy);

                if (xIsDigit && yIsDigit)
                {
                    // Compare full digit runs numerically
                    var xStart = i;
                    while (i < xLen && char.IsDigit(x[i])) i++;
                    var yStart = j;
                    while (j < yLen && char.IsDigit(y[j])) j++;

                    var xNumStr = x.Substring(xStart, i - xStart);
                    var yNumStr = y.Substring(yStart, j - yStart);

                    if (long.TryParse(xNumStr, out var xNum) && long.TryParse(yNumStr, out var yNum))
                    {
                        var numCompare = xNum.CompareTo(yNum);
                        if (numCompare != 0)
                        {
                            return numCompare;
                        }
                    }
                    else
                    {
                        // Fallback to string compare if parsing fails
                        var sCompare = string.Compare(xNumStr, yNumStr, StringComparison.OrdinalIgnoreCase);
                        if (sCompare != 0)
                        {
                            return sCompare;
                        }
                    }
                }
                else
                {
                    // Compare non-digit chars case-insensitively
                    var cCompare = char.ToUpperInvariant(cx).CompareTo(char.ToUpperInvariant(cy));
                    if (cCompare != 0)
                    {
                        return cCompare;
                    }

                    i++;
                    j++;
                }
            }

            // If all matched so far, shorter string comes first
            return xLen.CompareTo(yLen);
        }
    }
}