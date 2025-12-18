using Cysharp.Threading.Tasks;
using Foxbyte.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace FHH.Logic.Components.Networking
{
    public interface ICommandBus : IAppServiceAsync
    {
        event Action<bool> MuteAllChanged;
        event Action<Vector3> IndicatorPositionChanged;
        event Action<string, string, bool> GuidedModeChanged;
        event Action<string> VariantChanged;
        event Action<double3, Quaternion> TeleportOccurred;
        event Action<int, int, int, int, int> SunTimeChanged;

        void EnqueueMuteAll(bool value);
        void EnqueueIndicatorPosition(Vector3 position);
        void EnqueueGuidedMode(string projectId, string variant, bool enabled);
        void EnqueueVariant(string variant);
        void EnqueueTeleport(double3 position, Quaternion rotation);
        void EnqueueSunTime(int year, int month, int day, int hour, int minute); 
    }

    public class CommandBusService : ICommandBus
    {
        [SerializeField] private float _defaultThrottleSeconds = 2f;
        [SerializeField] private float _guidedModeThrottleSeconds = 5f;

        private const int TickIntervalMs = 100;

        public event Action<bool> MuteAllChanged;
        public event Action<Vector3> IndicatorPositionChanged;
        public event Action<string, string, bool> GuidedModeChanged;
        public event Action<string> VariantChanged;
        public event Action<double3, Quaternion> TeleportOccurred;
        public event Action<int, int, int, int, int> SunTimeChanged;

        // MuteAll state
        private bool _hasLastMuteAll;
        private bool _lastMuteAllValue;
        private float _lastMuteAllTime = float.NegativeInfinity;
        private bool _hasPendingMuteAll;
        private bool _pendingMuteAllValue;

        // Indicator state
        private bool _hasLastIndicator;
        private Vector3 _lastIndicatorPosition;
        private float _lastIndicatorTime = float.NegativeInfinity;
        private bool _hasPendingIndicator;
        private Vector3 _pendingIndicatorPosition;

        // GuidedMode state
        private bool _hasLastGuided;
        private string _lastGuidedProjectId;
        private string _lastGuidedVariant;
        private bool _lastGuidedEnabled;
        private float _lastGuidedTime = float.NegativeInfinity;
        private bool _hasPendingGuided;
        private string _pendingGuidedProjectId;
        private string _pendingGuidedVariant;
        private bool _pendingGuidedEnabled;

        // Variant state
        private bool _hasLastVariant;
        private string _lastVariant;
        private float _lastVariantTime = float.NegativeInfinity;
        private bool _hasPendingVariant;
        private string _pendingVariant;

        // Teleport state
        private bool _hasLastTeleport;
        private double3 _lastTeleportPosition;
        private float _lastTeleportTime = float.NegativeInfinity;
        private bool _hasPendingTeleport;
        private double3 _pendingTeleportPosition;
        private Quaternion _pendingTeleportRotation;

        // SunTime state
        private struct SunTimeState
        {                          
            public int Year;
            public int Month;
            public int Day;
            public int Hour;
            public int Minute;

            public SunTimeState(int year, int month, int day, int hour, int minute)
            {
                Year = year;                         
                Month = month;                       
                Day = day;                           
                Hour = hour;                         
                Minute = minute;                     
            }                                        

            public bool Equals(SunTimeState other)
            {
                return Year == other.Year &&         
                       Month == other.Month &&       
                       Day == other.Day &&           
                       Hour == other.Hour &&         
                       Minute == other.Minute;       
            }                                        
        }                                           

        private bool _hasLastSunTime;
        private SunTimeState _lastSunTime;
        private float _lastSunTimeTimestamp = float.NegativeInfinity;
        private bool _hasPendingSunTime;
        private SunTimeState _pendingSunTime;


        private CancellationTokenSource _cts;

        public async UniTask InitServiceAsync()
        {
            InitializeAsync().Forget();
            await UniTask.CompletedTask;
        }

        public async UniTask DisposeServiceAsync()
        {
            await ShutdownAsync();
        }

        public async UniTask InitializeAsync()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }

            _cts = new CancellationTokenSource();

            ULog.Info("CommandBusService.InitializeAsync: starting tick loop.");

            // start background UniTask loop
            TickLoop(_cts.Token).Forget();

            await UniTask.CompletedTask;
        }

        public async UniTask ShutdownAsync()
        {
            if (_cts != null)
            {
                ULog.Info("CommandBusService.ShutdownAsync: stopping tick loop.");
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }

            await UniTask.CompletedTask;
        }

        // UniTask loop calling Tick every 100 ms
        private async UniTask TickLoop(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    TickInternal();
                    await UniTask.Delay(TickIntervalMs, DelayType.UnscaledDeltaTime, PlayerLoopTiming.Update, token);
                }
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown
                ULog.Info("CommandBusService.TickLoop: canceled.");
            }
            catch (Exception ex)
            {
                ULog.Error($"CommandBusService.TickLoop: exception {ex}");
            }
        }

        private void TickInternal()
        {
            float now = Time.unscaledTime;

            // MuteAll
            if (_hasPendingMuteAll && now - _lastMuteAllTime >= _defaultThrottleSeconds)
            {
                ULog.Info($"CommandBusService.TickInternal: applying pending MuteAll={_pendingMuteAllValue}.");
                ApplyMuteAll(_pendingMuteAllValue, now);
                _hasPendingMuteAll = false;
            }

            // Indicator
            if (_hasPendingIndicator && now - _lastIndicatorTime >= _defaultThrottleSeconds)
            {
                ULog.Info(
                    $"CommandBusService.TickInternal: applying pending Indicator position={_pendingIndicatorPosition}.");
                ApplyIndicator(_pendingIndicatorPosition, now);
                _hasPendingIndicator = false;
            }

            // GuidedMode
            if (_hasPendingGuided && now - _lastGuidedTime >= _guidedModeThrottleSeconds)
            {
                ULog.Info(
                    $"CommandBusService.TickInternal: applying pending GuidedMode project={_pendingGuidedProjectId}, variant={_pendingGuidedVariant}, enabled={_pendingGuidedEnabled}.");
                ApplyGuidedMode(_pendingGuidedProjectId, _pendingGuidedVariant, _pendingGuidedEnabled, now);
                _hasPendingGuided = false;
            }
            // Variant
            if (_hasPendingVariant && now - _lastVariantTime >= _defaultThrottleSeconds)
            {
                ULog.Info($"CommandBusService.TickInternal: applying pending Variant={_pendingVariant}.");
                ApplyVariant(_pendingVariant, now);
                _hasPendingVariant = false;
            }                             

            // Teleport
            if (_hasPendingTeleport && now - _lastTeleportTime >= _defaultThrottleSeconds)
            {                                                                             
                ULog.Info($"CommandBusService.TickInternal: applying pending Teleport position={_pendingTeleportPosition}.");
                ApplyTeleport(_pendingTeleportPosition, _pendingTeleportRotation, now);
                _hasPendingTeleport = false;
            } 

            // SunTime
            if (_hasPendingSunTime && now - _lastSunTimeTimestamp >= _defaultThrottleSeconds)
            {
                ULog.Info("CommandBusService.TickInternal: applying pending SunTime " +  
                          $"year={_pendingSunTime.Year}, month={_pendingSunTime.Month}, day={_pendingSunTime.Day}, hour={_pendingSunTime.Hour}, minute={_pendingSunTime.Minute}.");
                ApplySunTime(
                    _pendingSunTime.Year,
                    _pendingSunTime.Month,
                    _pendingSunTime.Day,
                    _pendingSunTime.Hour,
                    _pendingSunTime.Minute,
                    now);                  
                _hasPendingSunTime = false; 
            }                                                                                 
        }

        public void EnqueueMuteAll(bool value)
        {
            float now = Time.unscaledTime;
            ProcessMuteAll(value, now);
        }

        public void EnqueueIndicatorPosition(Vector3 position)
        {
            float now = Time.unscaledTime;
            ProcessIndicator(position, now);
        }

        public void EnqueueGuidedMode(string projectId, string variant, bool enabled)
        {
            float now = Time.unscaledTime;
            ProcessGuidedMode(projectId, variant, enabled, now);
        }

        public void EnqueueVariant(string variant)
        {
            float now = Time.unscaledTime;
            ProcessVariant(variant, now);
        } 

        public void EnqueueTeleport(double3 position, Quaternion rotation)          
        {                                                      
            float now = Time.unscaledTime;                     
            ProcessTeleport(position, rotation, now);                    
        }                                                      

        public void EnqueueSunTime(int year, int month, int day, int hour, int minute)
        {
            float now = Time.unscaledTime; 
            ProcessSunTime(year, month, day, hour, minute, now);
        }   

        private void ProcessMuteAll(bool value, float now)
        {
            bool hasTarget = _hasPendingMuteAll || _hasLastMuteAll;
            bool currentTarget = _hasPendingMuteAll ? _pendingMuteAllValue : _lastMuteAllValue;

            if (hasTarget && currentTarget == value)
            {
                ULog.Info($"CommandBusService.ProcessMuteAll: discard duplicate value={value}.");
                return;
            }

            if (!_hasLastMuteAll || now - _lastMuteAllTime >= _defaultThrottleSeconds)
            {
                ULog.Info($"CommandBusService.ProcessMuteAll: applying immediately value={value}.");
                ApplyMuteAll(value, now);
            }
            else
            {
                ULog.Info($"CommandBusService.ProcessMuteAll: throttled, storing pending value={value}.");
                _pendingMuteAllValue = value;
                _hasPendingMuteAll = true;
            }
        }

        private void ApplyMuteAll(bool value, float now)
        {
            _hasLastMuteAll = true;
            _lastMuteAllValue = value;
            _lastMuteAllTime = now;

            ULog.Info($"CommandBusService.ApplyMuteAll: MuteAllChanged -> {value}.");
            MuteAllChanged?.Invoke(value);
        }

        private void ProcessIndicator(Vector3 position, float now)
        {
            bool hasTarget = _hasPendingIndicator || _hasLastIndicator;
            Vector3 currentTarget = _hasPendingIndicator ? _pendingIndicatorPosition : _lastIndicatorPosition;

            if (hasTarget && ArePositionsEqual(currentTarget, position))
            {
                ULog.Info("CommandBusService.ProcessIndicator: discard duplicate position.");
                return;
            }

            if (!_hasLastIndicator || now - _lastIndicatorTime >= _defaultThrottleSeconds)
            {
                ULog.Info($"CommandBusService.ProcessIndicator: applying immediately position={position}.");
                ApplyIndicator(position, now);
            }
            else
            {
                ULog.Info($"CommandBusService.ProcessIndicator: throttled, storing pending position={position}.");
                _pendingIndicatorPosition = position;
                _hasPendingIndicator = true;
            }
        }

        private void ApplyIndicator(Vector3 position, float now)
        {
            _hasLastIndicator = true;
            _lastIndicatorPosition = position;
            _lastIndicatorTime = now;

            ULog.Info($"CommandBusService.ApplyIndicator: IndicatorPositionChanged -> {position}.");
            IndicatorPositionChanged?.Invoke(position);
        }

        private void ProcessGuidedMode(string projectId, string variant, bool enabled, float now)
        {
            bool hasTarget = _hasPendingGuided || _hasLastGuided;
            if (hasTarget)
            {
                string currentProjectId = _hasPendingGuided ? _pendingGuidedProjectId : _lastGuidedProjectId;
                string currentVariant = _hasPendingGuided ? _pendingGuidedVariant : _lastGuidedVariant;
                bool currentEnabled = _hasPendingGuided ? _pendingGuidedEnabled : _lastGuidedEnabled;

                if (string.Equals(currentProjectId, projectId, StringComparison.Ordinal) &&
                    currentVariant == variant &&
                    currentEnabled == enabled)
                {
                    ULog.Info("CommandBusService.ProcessGuidedMode: discard duplicate GuidedMode state.");
                    return;
                }
            }

            if (!_hasLastGuided || now - _lastGuidedTime >= _guidedModeThrottleSeconds)
            {
                ULog.Info(
                    $"CommandBusService.ProcessGuidedMode: applying immediately project={projectId}, variant={variant}, enabled={enabled}.");
                ApplyGuidedMode(projectId, variant, enabled, now);
            }
            else
            {
                ULog.Info(
                    $"CommandBusService.ProcessGuidedMode: throttled, storing pending project={projectId}, variant={variant}, enabled={enabled}.");
                _pendingGuidedProjectId = projectId;
                _pendingGuidedVariant = variant;
                _pendingGuidedEnabled = enabled;
                _hasPendingGuided = true;
            }
        }

        private void ApplyGuidedMode(string projectId, string variant, bool enabled, float now)
        {
            _hasLastGuided = true;
            _lastGuidedProjectId = projectId;
            _lastGuidedVariant = variant;
            _lastGuidedEnabled = enabled;
            _lastGuidedTime = now;

            ULog.Info(
                $"CommandBusService.ApplyGuidedMode: GuidedModeChanged -> project={projectId}, variant={variant}, enabled={enabled}.");
            GuidedModeChanged?.Invoke(projectId, variant, enabled);
        }

        private void ProcessVariant(string variant, float now)
        {
            bool hasTarget = _hasPendingVariant || _hasLastVariant;
            string currentTarget = _hasPendingVariant ? _pendingVariant : _lastVariant;

            if (hasTarget && string.Equals(currentTarget, variant, StringComparison.Ordinal))
            {
                ULog.Info("CommandBusService.ProcessVariant: discard duplicate variant.");
                return;
            }

            if (!_hasLastVariant || now - _lastVariantTime >= _defaultThrottleSeconds)
            {
                ULog.Info($"CommandBusService.ProcessVariant: applying immediately variant={variant}.");
                ApplyVariant(variant, now);
            }
            else
            {
                ULog.Info($"CommandBusService.ProcessVariant: throttled, storing pending variant={variant}.");
                _pendingVariant = variant;
                _hasPendingVariant = true;
            }
        }

        private void ApplyVariant(string variant, float now)
        {                                                
            _hasLastVariant = true;                      
            _lastVariant = variant;                      
            _lastVariantTime = now;                      

            ULog.Info($"CommandBusService.ApplyVariant: VariantChanged -> {variant}.");
            VariantChanged?.Invoke(variant);  
        }

        // Teleport processing
        private void ProcessTeleport(double3 position, Quaternion rotation, float now)
        {
            bool hasTarget = _hasPendingTeleport || _hasLastTeleport;
            double3 currentTarget = _hasPendingTeleport ? _pendingTeleportPosition : _lastTeleportPosition;

            if (!_hasLastTeleport || now - _lastTeleportTime >= _defaultThrottleSeconds)
            { 
                ULog.Info($"CommandBusService.ProcessTeleport: applying immediately position={position}.");
                ApplyTeleport(position, rotation, now);
            }                                
            else                             
            {                                
                ULog.Info($"CommandBusService.ProcessTeleport: throttled, storing pending position={position}.");
                _pendingTeleportPosition = position; 
                _pendingTeleportRotation = rotation;
                _hasPendingTeleport = true; 
            } 
        }  

        private void ApplyTeleport(double3 position, Quaternion rotation, float now)
        {
            _hasLastTeleport = true;           
            _lastTeleportPosition = position;  
            _lastTeleportTime = now;           

            ULog.Info($"CommandBusService.ApplyTeleport: TeleportOccurred -> {position}.");
            TeleportOccurred?.Invoke(position, rotation);
        }

        // SunTime processing
        private void ProcessSunTime(int year, int month, int day, int hour, int minute, float now)
        {
            bool hasTarget = _hasPendingSunTime || _hasLastSunTime; 
            if (hasTarget) 
            {
                SunTimeState currentTarget = _hasPendingSunTime ? _pendingSunTime : _lastSunTime;
                SunTimeState incoming = new SunTimeState(year, month, day, hour, minute);

                if (currentTarget.Equals(incoming)) 
                {
                    ULog.Info("CommandBusService.ProcessSunTime: discard duplicate SunTime.");
                    return;
                }
            }

            if (!_hasLastSunTime || now - _lastSunTimeTimestamp >= _defaultThrottleSeconds)
            {
                ULog.Info($"CommandBusService.ProcessSunTime: applying immediately year={year}, month={month}, day={day}, hour={hour}, minute={minute}."); // ADDED
                ApplySunTime(year, month, day, hour, minute, now);
            }
            else
            {
                ULog.Info($"CommandBusService.ProcessSunTime: throttled, storing pending year={year}, month={month}, day={day}, hour={hour}, minute={minute}."); // ADDED
                _pendingSunTime = new SunTimeState(year, month, day, hour, minute);
                _hasPendingSunTime = true;
            }
        }

        private void ApplySunTime(int year, int month, int day, int hour, int minute, float now) 
        {
            _hasLastSunTime = true;
            _lastSunTime = new SunTimeState(year, month, day, hour, minute);
            _lastSunTimeTimestamp = now;

            ULog.Info($"CommandBusService.ApplySunTime: SunTimeChanged -> year={year}, month={month}, day={day}, hour={hour}, minute={minute}."); // ADDED
            SunTimeChanged?.Invoke(year, month, day, hour, minute);
        }  

        private bool ArePositionsEqual(Vector3 a, Vector3 b, float epsilon = 0.0001f)
        {
            return Mathf.Abs(a.x - b.x) < epsilon &&
                   Mathf.Abs(a.y - b.y) < epsilon &&
                   Mathf.Abs(a.z - b.z) < epsilon;
        }
    }
}