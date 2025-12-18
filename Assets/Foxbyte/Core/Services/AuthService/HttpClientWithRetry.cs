using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Foxbyte.Core.Services.DataTransfer;
using UnityEngine;
using UnityEngine.Networking;

namespace Foxbyte.Core.Services.AuthService
{
    public class HttpClientWithRetry : IHttpClient
    {
        private readonly int _maxRetries;
        private readonly TimeSpan _initialRetryDelay;
        private readonly TimeSpan _maxRetryDelay;

        public HttpClientWithRetry(int maxRetries = 3, int initialRetryDelayMs = 1000, int maxRetryDelayMs = 30000)
        {
            _maxRetries = maxRetries;
            _initialRetryDelay = TimeSpan.FromMilliseconds(initialRetryDelayMs);
            _maxRetryDelay = TimeSpan.FromMilliseconds(maxRetryDelayMs);
        }

        public async UniTask<HttpClientResult> GetAsync(string url, Dictionary<string, string> headers = null,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                using var request = UnityWebRequest.Get(url);
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.SetRequestHeader(header.Key, header.Value);
                    }
                }

                //request.SetRequestHeader("Accept", "application/json");
                await request.SendWebRequest().WithCancellation(cancellationToken);
                if (request.result != UnityWebRequest.Result.Success)
                {
                    return new HttpClientResult
                    {
                        IsSuccess = false,
                        StatusCode = request.responseCode,
                        ErrorMessage = $"UnityWebRequest failed: {request.error}"
                    };
                }

                return new HttpClientResult
                {
                    IsSuccess = true,
                    StatusCode = request.responseCode,
                    Data = request.downloadHandler.text
                };
            });
        }

        public async UniTask<HttpClientResult> PostAsync(
            string url,
            Dictionary<string, string> parameters = null,
            string jsonData = null,
            Dictionary<string, string> headers = null,
            CancellationToken cancellationToken = default
        )
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                UnityWebRequest request;

                if (jsonData != null)
                {
                    request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
                    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.SetRequestHeader("Content-Type", "application/json");
                }
                else if (parameters != null)
                {
                    string body = BuildFormUrlEncoded(parameters);
                    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);
                    request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                    //request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                    //ULog.Info($"[HTTP] POST to: {url}");
                    //ULog.Info($"[HTTP] Body: {body}");
                    //ULog.Info($"[HTTP] Body length: {bodyRaw.Length} bytes");
                }
                else
                {
                    //return new HttpClientResult
                    //{
                    //    IsSuccess = false,
                    //    ErrorMessage = "Either jsonData or parameters must be provided."
                    //};
                    // Allow empty-body POST when only headers are provided (e.g., logout endpoints)
                    request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
                    request.uploadHandler = new UploadHandlerRaw(Array.Empty<byte>());
                }

                request.downloadHandler = new DownloadHandlerBuffer();

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.SetRequestHeader(header.Key, header.Value);
                    }
                }

                //request.chunkedTransfer = false;
                request.useHttpContinue = false;
                
                // Prevent auto-follow that can drop Authorization headers on redirect
                //request.redirectLimit = 0;

                try
                {
                    await request.SendWebRequest().WithCancellation(cancellationToken);
                }
                catch (UnityWebRequestException uex)
                {
                    // Convert exception to a normal result so the outer retry loop doesn't log "Max retries..."
                    var body = request.downloadHandler != null ? request.downloadHandler.text : null;
                    return new HttpClientResult
                    {
                        IsSuccess = false,
                        StatusCode = request.responseCode != 0 ? request.responseCode : (long)uex.ResponseCode,
                        ErrorMessage = uex.Message,
                        Data = body // keep body so callers (refresh) can inspect {"error":"invalid_grant"}
                    };
                }

                //ULog.Info($"[HTTP] Response Code: {request.responseCode}");
                //ULog.Info($"[HTTP] Response: {request.downloadHandler.text}");

                if (request.result != UnityWebRequest.Result.Success)
                {
                    string errorDetails = $"Status: {request.responseCode}, Error: {request.error}";
                    if (!string.IsNullOrEmpty(request.downloadHandler?.text))
                    {
                        errorDetails += $"\nResponse Body: {request.downloadHandler.text}";
                    }

                    Debug.LogError($"[HttpClientWithRetry] Request failed: {errorDetails}");

                    return new HttpClientResult
                    {
                        IsSuccess = false,
                        StatusCode = request.responseCode,
                        ErrorMessage = $"UnityWebRequest failed: {request.error}"
                    };
                }

                return new HttpClientResult
                {
                    IsSuccess = true,
                    StatusCode = request.responseCode,
                    Data = request.downloadHandler.text
                };
            });
        }

        /// <summary>
        /// Post without body. Overload to do empty POST.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async UniTask<HttpClientResult> PostAsync(
            string url,
            Dictionary<string, string> headers,
            CancellationToken cancellationToken = default
        )
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
                // zero-length body is fine for many logout endpoints
                request.uploadHandler = new UploadHandlerRaw(Array.Empty<byte>());
                request.downloadHandler = new DownloadHandlerBuffer();

                if (headers != null)
                {
                    foreach (var header in headers)
                        request.SetRequestHeader(header.Key, header.Value);
                }

                //request.chunkedTransfer = false;
                request.useHttpContinue = false;
                
                try
                {
                    await request.SendWebRequest().WithCancellation(cancellationToken);
                }
                catch (UnityWebRequestException uex)
                {
                    // Convert exception to a normal result so the outer retry loop doesn't log "Max retries..."
                    var body = request.downloadHandler != null ? request.downloadHandler.text : null;
                    return new HttpClientResult
                    {
                        IsSuccess = false,
                        StatusCode = request.responseCode != 0 ? request.responseCode : (long)uex.ResponseCode,
                        ErrorMessage = uex.Message,
                        Data = body // keep body so callers (refresh) can inspect {"error":"invalid_grant"}
                    };
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    return new HttpClientResult
                    {
                        IsSuccess = false,
                        StatusCode = request.responseCode,
                        ErrorMessage = $"UnityWebRequest failed: {request.error}",
                        Data = request.downloadHandler?.text
                    };
                }

                return new HttpClientResult
                {
                    IsSuccess = true,
                    StatusCode = request.responseCode,
                    Data = request.downloadHandler.text
                };
            });
        }


        private static string BuildFormUrlEncoded(Dictionary<string, string> parameters)
        {
            // Use strict RFC3986 encoding (Uri.EscapeDataString), join with &
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            bool first = true;
            foreach (var kv in parameters)
            {
                if (!first) sb.Append('&');
                first = false;
                sb.Append(Uri.EscapeDataString(kv.Key));
                sb.Append('=');
                sb.Append(Uri.EscapeDataString(kv.Value ?? string.Empty));
            }

            return sb.ToString();
        }

        private async UniTask<HttpClientResult> ExecuteWithRetryAsync(Func<UniTask<HttpClientResult>> action)
        {
            int retryCount = 0;
            TimeSpan delay = _initialRetryDelay;

            while (true)
            {
                try
                {
                    var result = await action();

                    if (!result.IsSuccess && result.StatusCode == 400)
                    {
                        ULog.Warning($"[HTTP] 400 Bad Request - NOT retrying: {result.ErrorMessage}");
                        return result;
                    }

                    if (!result.IsSuccess && retryCount < _maxRetries)
                    {
                        retryCount++;
                        ULog.Error(
                            $"[HTTP] Request failed. Retrying in {delay.TotalSeconds} seconds. Retry count: {retryCount}");
                        await UniTask.Delay(delay, cancellationToken: CancellationToken.None);
                        delay = CalculateNextDelay(delay);
                    }
                    else
                    {
                        return result;
                    }
                }
                catch (OperationCanceledException)
                {
                    ULog.Error("Request canceled.");
                    return new HttpClientResult { IsSuccess = false, ErrorMessage = "Request canceled" };
                }
                catch (Exception ex)
                {
                    if (retryCount >= _maxRetries || !ShouldRetry(ex))
                    {
                        ULog.Error($"Max retries reached or non-retryable error: {ex}");
                        return new HttpClientResult { IsSuccess = false, ErrorMessage = ex.Message };
                    }

                    retryCount++;
                    ULog.Error(
                        $"[HTTP] Request failed. Retrying in {delay.TotalSeconds} seconds. Retry count: {retryCount}");
                    await UniTask.Delay(delay, cancellationToken: CancellationToken.None);
                    delay = CalculateNextDelay(delay);
                }
            }
        }

        private bool ShouldRetry(Exception ex)
        {
            // Add logic to determine if the exception is retryable
            return ex.Message.Contains("UnityWebRequest failed");
        }

        private TimeSpan CalculateNextDelay(TimeSpan currentDelay)
        {
            var nextDelay = TimeSpan.FromMilliseconds(currentDelay.TotalMilliseconds * 2);
            return nextDelay > _maxRetryDelay ? _maxRetryDelay : nextDelay;
        }

        public async UniTask<IDisposable> SubscribeAsync(
            string url,
            Dictionary<string, string> headers = null,
            Action<string> onData = null,
            CancellationToken cancellationToken = default
        )
        {
            var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
            request.downloadHandler = new SseDownloadHandler(onData);
            request.SetRequestHeader("Accept", "text/event-stream");
            request.SetRequestHeader("Cache-Control", "no-cache");
            //request.SetRequestHeader("Connection", "keep-alive");
            request.redirectLimit = 0; // keep Authorization on redirect by preventing it

            if (headers != null)
            {
                foreach (var header in headers)
                    request.SetRequestHeader(header.Key, header.Value);
            }

            var asyncOperation = request.SendWebRequest();
            var ctr = cancellationToken.Register(() => request.Abort());

            // Wait until headers arrive (responseCode becomes non-zero) or we receive first bytes
            var startedAt = DateTime.UtcNow;
            try
            {
                await UniTask.WaitUntil(() =>
                        request.isDone ||
                        request.downloadedBytes > 0 ||
                        request.responseCode != 0,
                    cancellationToken: cancellationToken
                );
            }
            catch (OperationCanceledException)
            {
                ctr.Dispose();
                request.Dispose();
                throw; // propagate cancellation
            }

            // If server responded already, validate status and content-type
            if (request.responseCode != 0)
            {
                var status = (int)request.responseCode;
                var ctHeader = request.GetResponseHeader("Content-Type") ?? "";

                if (status == 401 || status == 403)
                {
                    ctr.Dispose();
                    request.Dispose();
                    // Surface a specific exception so the caller can refresh tokens only when needed
                    throw new AuthRequiredException($"SSE unauthorized: HTTP {status}");
                }

                if (status < 200 || status >= 300)
                {
                    ctr.Dispose();
                    request.Dispose();
                    throw new Exception($"Failed to establish SSE connection: HTTP {status} ({request.error})");
                }

                // Optional: enforce event-stream content type if your backend sets it
                if (!ctHeader.StartsWith("text/event-stream", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.LogWarning($"[HTTP] SSE connected but Content-Type='{ctHeader}'");
                }
            }

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                var status = (int)request.responseCode;
                ctr.Dispose();
                request.Dispose();
                if (status == 401 || status == 403)
                    throw new AuthRequiredException($"SSE unauthorized: HTTP {status}");
                throw new Exception($"Failed to establish SSE connection: {request.error} (HTTP {status})");
            }

            // Success: streaming is in progress
            Debug.Log($"[HTTP] SSE connected in {(int)(DateTime.UtcNow - startedAt).TotalMilliseconds}ms");
            return new RequestSubscription(request, ctr);
        }
    
        public sealed class AuthRequiredException : Exception
        {
            public AuthRequiredException(string message) : base(message) { }
        }

        // handle both request and cancellation token registration
        private class RequestSubscription : IDisposable
        {
            private UnityWebRequest _request;
            private CancellationTokenRegistration _ctr;
            private bool _disposed = false;

            public RequestSubscription(UnityWebRequest request, CancellationTokenRegistration ctr)
            {
                _request = request;
                _ctr = ctr;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _request?.Abort();
                    _request?.Dispose();
                    _ctr.Dispose();
                    _disposed = true;
                }
            }
        }

        public class HttpClientResult
        {
            public bool IsSuccess { get; set; }
            public long StatusCode { get; set; }
            public string Data { get; set; }
            public string ErrorMessage { get; set; }
        }


        /// <summary>
        /// Get binary data (e.g. images, files) with retry logic.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async UniTask<HttpClientBinaryResult> GetBytesAsync(
            string url,
            Dictionary<string, string> headers = null,
            CancellationToken cancellationToken = default
        )
        {
            return await ExecuteWithRetryBytesAsync(async () =>
            {
                using var request = UnityWebRequest.Get(url);
                request.downloadHandler = new DownloadHandlerBuffer();

                if (headers != null)
                {
                    foreach (var header in headers)
                        request.SetRequestHeader(header.Key, header.Value);
                }

                try
                {
                    await request.SendWebRequest().WithCancellation(cancellationToken);
                }
                catch (UnityWebRequestException uex)
                {
                    var bytes = request.downloadHandler != null ? request.downloadHandler.data : null;
                    return new HttpClientBinaryResult
                    {
                        IsSuccess = false,
                        StatusCode = request.responseCode != 0 ? request.responseCode : (long)uex.ResponseCode,
                        ErrorMessage = uex.Message,
                        Bytes = bytes
                    };
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    return new HttpClientBinaryResult
                    {
                        IsSuccess = false,
                        StatusCode = request.responseCode,
                        ErrorMessage = $"UnityWebRequest failed: {request.error}",
                        Bytes = request.downloadHandler?.data
                    };
                }

                return new HttpClientBinaryResult
                {
                    IsSuccess = true,
                    StatusCode = request.responseCode,
                    Bytes = request.downloadHandler.data
                };
            });
        }

        private async UniTask<HttpClientBinaryResult> ExecuteWithRetryBytesAsync(
            Func<UniTask<HttpClientBinaryResult>> action
        )
        {
            int retryCount = 0;
            TimeSpan delay = _initialRetryDelay;

            while (true)
            {
                try
                {
                    var result = await action();

                    if (!result.IsSuccess && result.StatusCode == 400)
                    {
                        ULog.Warning($"[HTTP] 400 Bad Request (bytes) - NOT retrying: {result.ErrorMessage}");
                        return result;
                    }

                    if (!result.IsSuccess && retryCount < _maxRetries)
                    {
                        retryCount++;
                        ULog.Error($"[HTTP] Bytes request failed. Retrying in {delay.TotalSeconds} seconds. Retry count: {retryCount}");
                        await UniTask.Delay(delay, cancellationToken: CancellationToken.None);
                        delay = CalculateNextDelay(delay);
                    }
                    else
                    {
                        return result;
                    }
                }
                catch (OperationCanceledException)
                {
                    ULog.Error("Request canceled (bytes).");
                    return new HttpClientBinaryResult { IsSuccess = false, ErrorMessage = "Request canceled", Bytes = null };
                }
                catch (Exception ex)
                {
                    if (retryCount >= _maxRetries || !ShouldRetry(ex))
                    {
                        ULog.Error($"Max retries reached or non-retryable error (bytes): {ex.Message}");
                        return new HttpClientBinaryResult { IsSuccess = false, ErrorMessage = ex.Message, Bytes = null };
                    }

                    retryCount++;
                    ULog.Error($"[HTTP] Bytes request failed. Retrying in {delay.TotalSeconds} seconds. Retry count: {retryCount}");
                    await UniTask.Delay(delay, cancellationToken: CancellationToken.None);
                    delay = CalculateNextDelay(delay);
                }
            }
        }

        public class HttpClientBinaryResult
        {
            public bool IsSuccess { get; set; }
            public long StatusCode { get; set; }
            public byte[] Bytes { get; set; }
            public string ErrorMessage { get; set; }
        }


        public async UniTask<HttpClientHeadResult> HeadAsync(
            string url,
            Dictionary<string, string> headers = null,
            CancellationToken cancellationToken = default
        )
        {
            return await ExecuteWithRetryHeadAsync(async () =>
            {
                using var request = UnityWebRequest.Head(url);
                if (headers != null)
                {
                    foreach (var header in headers)
                        request.SetRequestHeader(header.Key, header.Value);
                }

                try
                {
                    await request.SendWebRequest().WithCancellation(cancellationToken);
                }
                catch (UnityWebRequestException uex)
                {
                    return new HttpClientHeadResult
                    {
                        IsSuccess = false,
                        StatusCode = request.responseCode != 0 ? request.responseCode : (long)uex.ResponseCode,
                        ErrorMessage = uex.Message,
                        ContentLength = null
                    };
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    return new HttpClientHeadResult
                    {
                        IsSuccess = false,
                        StatusCode = request.responseCode,
                        ErrorMessage = $"UnityWebRequest failed: {request.error}",
                        ContentLength = null
                    };
                }

                long? len = null;
                try
                {
                    var cl = request.GetResponseHeader("Content-Length");
                    if (!string.IsNullOrEmpty(cl) && long.TryParse(cl, out var parsed))
                        len = parsed;
                }
                catch { }

                return new HttpClientHeadResult
                {
                    IsSuccess = true,
                    StatusCode = request.responseCode,
                    ContentLength = len
                };
            });
        }

        private async UniTask<HttpClientHeadResult> ExecuteWithRetryHeadAsync(Func<UniTask<HttpClientHeadResult>> action)
        {
            int retryCount = 0;
            TimeSpan delay = _initialRetryDelay;

            while (true)
            {
                try
                {
                    var result = await action();

                    if (!result.IsSuccess && result.StatusCode == 400)
                    {
                        ULog.Warning($"[HTTP] 400 Bad Request (HEAD) - NOT retrying: {result.ErrorMessage}");
                        return result;
                    }

                    if (!result.IsSuccess && retryCount < _maxRetries)
                    {
                        retryCount++;
                        ULog.Error($"[HTTP] HEAD failed. Retrying in {delay.TotalSeconds} seconds. Retry count: {retryCount}");
                        await UniTask.Delay(delay, cancellationToken: CancellationToken.None);
                        delay = CalculateNextDelay(delay);
                    }
                    else
                    {
                        return result;
                    }
                }
                catch (OperationCanceledException)
                {
                    ULog.Error("HEAD canceled.");
                    return new HttpClientHeadResult { IsSuccess = false, ErrorMessage = "Request canceled" };
                }
                catch (Exception ex)
                {
                    if (retryCount >= _maxRetries || !ShouldRetry(ex))
                    {
                        ULog.Error($"HEAD max retries reached or non-retryable error: {ex.Message}");
                        return new HttpClientHeadResult { IsSuccess = false, ErrorMessage = ex.Message };
                    }

                    retryCount++;
                    ULog.Error($"[HTTP] HEAD failed. Retrying in {delay.TotalSeconds} seconds. Retry count: {retryCount}");
                    await UniTask.Delay(delay, cancellationToken: CancellationToken.None);
                    delay = CalculateNextDelay(delay);
                }
            }
        }

        public class HttpClientHeadResult
        {
            public bool IsSuccess { get; set; }
            public long StatusCode { get; set; }
            public string ErrorMessage { get; set; }
            public long? ContentLength { get; set; }
        }
    }
}