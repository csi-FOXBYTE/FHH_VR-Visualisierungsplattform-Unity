using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Foxbyte.Core.Services.AuthService
{
    public sealed class LoopbackRedirectListener : IDisposable
    {
        private TcpListener _listener;
        private CancellationTokenSource _cts;
        private UniTaskCompletionSource<string> _tcs;
        private string _redirectUri;

        public string RedirectUri => _redirectUri;

        // Call with preferred fixed ports, e.g., new[] { 48152, 48153, 48154 }
        public void Start(IEnumerable<int> candidatePorts, string path = "/callback", string expectedRedirectUri = null)

        {
            _tcs = new UniTaskCompletionSource<string>();
            _cts = new CancellationTokenSource();

            int boundPort = BindFirstAvailable(candidatePorts);
        
            _redirectUri = !string.IsNullOrEmpty(expectedRedirectUri)
                ? expectedRedirectUri
                : $"http://localhost:{boundPort}{NormalizePath(path)}";


            AcceptOnceAsync(_cts.Token).Forget();
        }

        public UniTask<string> WaitForCallbackAsync(TimeSpan timeout)
        {
            return _tcs.Task.Timeout(timeout);
        }

        public void Dispose()
        {
            try { _cts?.Cancel(); } catch { }
            try { _listener?.Stop(); } catch { }
            _cts?.Dispose();
        }

        private int BindFirstAvailable(IEnumerable<int> ports)
        {
            foreach (int port in ports)
            {
                try
                {
                    _listener = new TcpListener(IPAddress.Loopback, port);
                    _listener.Start();
                    return port;
                }
                catch (SocketException)
                {
                    // Port in use -> try next
                    try { _listener?.Stop(); } catch { }
                    _listener = null;
                }
            }
            throw new InvalidOperationException("No candidate loopback ports available.");
        }

        private async UniTaskVoid AcceptOnceAsync(CancellationToken token)
        {
            try
            {
                using (var client = await _listener.AcceptTcpClientAsync())
                using (var stream = client.GetStream())
                using (var reader = new StreamReader(stream, Encoding.ASCII, false, 1024, true))
                {
                    string requestLine = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(requestLine))
                    {
                        await WriteResponseAsync(stream, 400, "<p>Bad Request</p>");
                        _tcs.TrySetException(new Exception("Empty request."));
                        return;
                    }

                    string line;
                    while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync()))
                    {
                        if (token.IsCancellationRequested) return;
                    }

                    string[] parts = requestLine.Split(' ');
                    string pathAndQuery = parts.Length >= 2 ? parts[1] : "/";
                    var fullUri = new Uri($"http://localhost{pathAndQuery}");

                    //ULog.Info($"[Loopback] Received request: {pathAndQuery}");
                    //ULog.Info($"[Loopback] Query string: {fullUri.Query}");

                    await WriteResponseAsync(stream, 200,
                        "<html><body><h3>Login abgeschlossen.</h3><p>Dieses Fenster kann geschlossen werden.</p><p>This window can be closed.</p></body></html>");

                    string finalUrl = $"{_redirectUri}{fullUri.Query}";
                    //ULog.Info($"[Loopback] Returning URL: {finalUrl}");
                    _tcs.TrySetResult(finalUrl);
                }
            }
            catch (Exception ex)
            {
                _tcs.TrySetException(ex);
            }
            finally
            {
                Dispose();
            }
        }

        private static async UniTask WriteResponseAsync(NetworkStream stream, int code, string html)
        {
            byte[] body = Encoding.UTF8.GetBytes(html);
            string header =
                $"HTTP/1.1 {code} OK\r\nContent-Type: text/html; charset=UTF-8\r\nContent-Length: {body.Length}\r\nConnection: close\r\n\r\n";
            byte[] headerBytes = Encoding.ASCII.GetBytes(header);
            await stream.WriteAsync(headerBytes, 0, headerBytes.Length);
            await stream.WriteAsync(body, 0, body.Length);
            await stream.FlushAsync();
        }

        private static string NormalizePath(string p)
        {
            if (string.IsNullOrEmpty(p)) return "/";
            if (!p.StartsWith("/")) p = "/" + p;
            if (p[p.Length - 1] == '/') p = p.Substring(0, p.Length - 1);
            return p;
        }
    }
}