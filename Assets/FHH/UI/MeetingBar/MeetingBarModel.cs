using System;
using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;

namespace FHH.UI.MeetingBar
{
    public sealed class MeetingBarModel : PresenterModelBase
    {
        public string ProjectId { get; private set; }
        public string ProjectName { get; private set; }

        public bool IsAdaptiveMicOn { get; private set; }
        public bool IsMicOn { get; private set; }
        public bool IsGuidedAccessOn { get; private set; }

        public string EventId { get; private set; } // authoritative id from CollaborationService
        public string Status  { get; private set; } // PLANNED/ACTIVE/MISSING_HOST/END
        public bool HasSessionStarted { get; private set; }

        private DateTime? _sessionStartUtc;

        public override async UniTask InitializeAsync()
        {
            await UniTask.CompletedTask;
        }

        public void SetProjectInfo(string projectId, string projectName)
        {
            ProjectId  = projectId;
            ProjectName = projectName;
        }

        // track current event id + status
        public void SetEvent(string eventId) => EventId = eventId;
        public void SetStatus(string status) => Status = status;

        // explicit clock control
        public void StartSessionClock()
        {
            if (_sessionStartUtc == null)
            {
                _sessionStartUtc = DateTime.UtcNow;
                HasSessionStarted = true;
            }
        }

        public void StopSessionClock()
        {
            _sessionStartUtc = null;
            HasSessionStarted = false;
        }

        public TimeSpan GetSessionDuration()
        {
            if (_sessionStartUtc == null) return TimeSpan.Zero;
            return DateTime.UtcNow - _sessionStartUtc.Value;
        }

        public void ToggleAdaptiveMic() => IsAdaptiveMicOn = !IsAdaptiveMicOn;
        public void ToggleMic() => IsMicOn = !IsMicOn;
        public void ToggleGuidedAccess() => IsGuidedAccessOn = !IsGuidedAccessOn;
    }
}