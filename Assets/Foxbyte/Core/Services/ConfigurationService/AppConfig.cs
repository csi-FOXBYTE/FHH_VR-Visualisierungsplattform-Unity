using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Foxbyte.Core
{
    public class AppConfig
    {
        public ServerConfig ServerConfig { get; set; }
        public LoggingConfig LoggingConfig { get; set; }
    }

    public class ServerConfig
    {
        public string ApiBaseUrl { get; set; }
        public string ApiVersion { get; set; }
        public string AuthEndpoint { get; set; }
        public string TokenEndpoint { get; set; }
        public string ClientId { get; set; }
        public string RedirectUri { get; set; }
        public string RedirectUriForBrowser { get; set; }
        public string RevocationEndpoint { get; set; }
        public string RefreshEndpoint { get; set; }
        public string LogoutEndpoint { get; set; }
        public string[] Scopes { get; set; }
        public Endpoint[] Endpoints { get; set; }
        public SseEndpoint[] SseEndpoints { get; set; }
        public Header[] Headers { get; set; }
        public string TilesBaseUrl { get; set; }
        public string AppVersion { get; set; }
    

        /// <summary>
        /// Combine ApiBaseUrl + optional ApiVersion + a relative path (template supported),
        /// then fill route values and query parameters.
        /// </summary>
        public string Url(
            string relativeTemplate,
            object routeValues = null,
            IDictionary<string, string> query = null,
            bool includeVersion = true)
        {
            var baseUrl = includeVersion && string.IsNullOrWhiteSpace(ApiVersion) == false
                ? _CombineUrl(ApiBaseUrl, ApiVersion)
                : ApiBaseUrl;

            var relative = _ResolveTemplate(relativeTemplate, _ObjectToDictionary(routeValues));
            var withBase = _CombineUrl(baseUrl, relative);
            return _AttachQuery(withBase, query);
        }

        /// <summary>
        /// Builds URL for a named Endpoint entry (e.g., Name = "EventStatus", Path = "events/{id}/status").
        /// </summary>
        public string EndpointUrl(
            string name,
            object routeValues = null,
            IDictionary<string, string> query = null,
            bool includeVersion = true)
        {
            var ep = Endpoints?.FirstOrDefault(e =>
                string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase));

            if (ep == null)
            {
                throw new InvalidOperationException($"Endpoint '{name}' not found.");
            }

            return Url(ep.Path, routeValues, query, includeVersion);
        }

        /// <summary>
        /// Convenience for the string properties already existing (e.g., Events, EventStatus, Project, etc.).
        /// Example: UrlFromProp(nameof(EventStatus), new { id = "123" })
        /// </summary>
        public string UrlFromProp(
            string propertyName,
            object routeValues = null,
            IDictionary<string, string> query = null,
            bool includeVersion = true)
        {
            var prop = typeof(ServerConfig).GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            if (prop == null || prop.PropertyType != typeof(string))
            {
                throw new InvalidOperationException($"String property '{propertyName}' not found on ServerConfig.");
            }

            var value = (string)prop.GetValue(this);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"Property '{propertyName}' is null/empty.");
            }

            return Url(value, routeValues, query, includeVersion);
        }

        /// <summary>
        /// Builds relative URL from a relative path (template supported),
        /// </summary>
        /// <param name="relativeTemplate"></param>
        /// <param name="routeValues"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public string RelativeUrl(
            string relativeTemplate,
            object routeValues = null,
            IDictionary<string, string> query = null)
        {
            var relative = _ResolveTemplate(relativeTemplate, _ObjectToDictionary(routeValues));
            relative = _NormalizeLeadingSlash(relative);
            return _AttachQuery(relative, query);
        }

        /// <summary>
        /// Builds relative URL for a named Endpoint entry (e.g., Name = "EventStatus", Path = "events/{id}/status").
        /// </summary>
        /// <param name="name"></param>
        /// <param name="routeValues"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public string EndpointPath(
            string name,
            object routeValues = null,
            IDictionary<string, string> query = null)
        {
            var ep = Endpoints?.FirstOrDefault(e =>
                string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase));

            if (ep == null)
            {
                throw new InvalidOperationException($"Endpoint '{name}' not found.");
            }

            return RelativeUrl(ep.Path, routeValues, query);
        }

        /// <summary>
        /// Convenience for the string properties already existing (e.g., Events, EventStatus, Project, etc.).
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="routeValues"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public string UrlFromPropPath(
            string propertyName,
            object routeValues = null,
            IDictionary<string, string> query = null)
        {
            var prop = typeof(ServerConfig).GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            if (prop == null || prop.PropertyType != typeof(string))
            {
                throw new InvalidOperationException($"String property '{propertyName}' not found on ServerConfig.");
            }

            var value = (string)prop.GetValue(this);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"Property '{propertyName}' is null/empty.");
            }

            return RelativeUrl(value, routeValues, query);
        }

        private static string _NormalizeLeadingSlash(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return "/";
            return path.StartsWith("/", StringComparison.Ordinal) ? path : "/" + path;
        }

        private static string _CombineUrl(string baseUrl, string relative)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("ApiBaseUrl must be set.");
            }

            // Ensure base has trailing slash for Uri combination.
            if (baseUrl.EndsWith("/") == false) baseUrl += "/";

            // Allow relative to omit leading slash.
            relative ??= string.Empty;

            var result = new Uri(new Uri(baseUrl, UriKind.Absolute), relative).ToString();
            return result;
        }

        private static string _ResolveTemplate(string template, IDictionary<string, string> values)
        {
            if (string.IsNullOrEmpty(template)) return template;

            if (values == null || values.Count == 0) return template;

            foreach (var kv in values)
            {
                var token = "{" + kv.Key + "}";
                if (template.Contains(token, StringComparison.Ordinal))
                {
                    template = template.Replace(token, Uri.EscapeDataString(kv.Value ?? string.Empty), StringComparison.Ordinal);
                }
            }

            // optional sanity-check for any unreplaced tokens like {id}
            // for strict behavior uncomment below:
            // if (template.IndexOf('{') >= 0 && template.IndexOf('}') > template.IndexOf('{'))
            //     throw new InvalidOperationException($"Unresolved route tokens in '{template}'.");

            return template;
        }

        private static string _AttachQuery(string url, IDictionary<string, string> query)
        {
            if (query == null || query.Count == 0) return url;

            var sb = new StringBuilder();
            sb.Append(url);
            sb.Append(url.Contains("?") ? "&" : "?");

            var first = true;
            foreach (var kv in query)
            {
                if (!first) sb.Append("&");
                first = false;
                sb.Append(Uri.EscapeDataString(kv.Key ?? string.Empty));
                sb.Append("=");
                sb.Append(Uri.EscapeDataString(kv.Value ?? string.Empty));
            }

            return sb.ToString();
        }

        private static IDictionary<string, string> _ObjectToDictionary(object obj)
        {
            if (obj == null) return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Already a dictionary<string,string>
            if (obj is IDictionary<string, string> dictStr) return new Dictionary<string, string>(dictStr, StringComparer.OrdinalIgnoreCase);

            // IDictionary<string, object>
            if (obj is IDictionary<string, object> dictObj)
            {
                return dictObj.ToDictionary(
                    kv => kv.Key,
                    kv => kv.Value?.ToString() ?? string.Empty,
                    StringComparer.OrdinalIgnoreCase);
            }

            // Anonymous or POCO: reflect public readable props
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var props = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var p in props)
            {
                if (!p.CanRead) continue;
                var val = p.GetValue(obj);
                result[p.Name] = val?.ToString() ?? string.Empty;
            }

            return result;
        }
    }

    public class Endpoint
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Method { get; set; } // GET, POST, PUT, DELETE, PATCH
        public string[] Parameters { get; set; } // Optional parameters for specific queries like type, status

    }

    public class SseEndpoint
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Method { get; set; } // GET, POST, PUT, DELETE, PATCH
        public string[] Parameters { get; set; } // Optional parameters for specific queries like type, status
        public string EventType { get; set; } // Type of event to listen for
    }

    public class Header
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class LoggingConfig
    {
        public string LogFileName { get; set; }
    }
}