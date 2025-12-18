using System;
using System.Text;
using UnityEngine.Networking;

namespace Foxbyte.Core.Services.DataTransfer
{
    internal class SseDownloadHandler : DownloadHandlerScript
    {
        private readonly Action<string> _onData;
        private readonly StringBuilder _buffer = new StringBuilder();

        public SseDownloadHandler(Action<string> onData) : base()
        {
            _onData = onData;
        }

        protected override bool ReceiveData(byte[] theData, int dataLength)
        {
            if (theData == null || dataLength == 0)
                return true;

            try
            {
                var chunk = Encoding.UTF8.GetString(theData, 0, dataLength);
                ProcessChunk(chunk);
            }
            catch (Exception ex)
            {
                ULog.Error($"Error processing SSE data: {ex.Message}");
            }

            return true;
        }

        private void ProcessChunk(string chunk)
        {
            var lines = chunk.Split(new[] { '\n' }, StringSplitOptions.None);
            
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].TrimEnd('\r'); // Handle both \n and \r\n
                
                if (string.IsNullOrEmpty(line))
                {
                    // Empty line indicates end of event
                    var eventData = _buffer.ToString().Trim();
                    _buffer.Clear();
                    
                    if (!string.IsNullOrEmpty(eventData))
                    {
                        ProcessEvent(eventData);
                    }
                }
                else
                {
                    _buffer.AppendLine(line);
                }
            }
        }

        private void ProcessEvent(string eventData)
        {
            if (eventData.StartsWith(":"))
            {
                // Comment/heartbeat - ignore
                return;
            }

            var lines = eventData.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var line in lines)
            {
                if (line.StartsWith("data:"))
                {
                    var json = line.Substring(5).Trim();
                    if (!string.IsNullOrEmpty(json))
                    {
                        _onData?.Invoke(json);
                    }
                }
                // handling of other SSE fields like event:, id:, retry: here if needed
            }
        }
    }
}