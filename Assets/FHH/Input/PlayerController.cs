using CesiumForUnity;
using Cysharp.Threading.Tasks;
using FHH.Logic;
using FHH.Logic.Cesium;
using FHH.Logic.Components.Collaboration;
using FHH.Logic.Components.HmdPresenceMonitor;
using FHH.Logic.Components.Networking;
using FHH.Logic.Models;
using FHH.Logic.VR;
using Foxbyte.Core;
using Foxbyte.Core.Services.ConfigurationService;
using Foxbyte.Core.Services.Permission;
using System;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

namespace FHH.Input
{
    [RequireComponent(typeof(XROrigin))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private ContinuousMoveProvider _controllerMove; // xr controller actions
        [SerializeField] private ContinuousMoveProvider _controllerFly; // xr controller actions
        [SerializeField] private ContinuousMoveProvider _keyboardMove; // player actions
        [SerializeField] private ContinuousMoveProvider _keyboardFly; // player actions
        [SerializeField] private SnapTurnProvider _snapTurn;
        [SerializeField] private ContinuousTurnProvider _turn;
        [SerializeField] private GravityProvider _gravityProvider;
        //[SerializeField] private TeleportationProvider _teleport;
        [SerializeField] private MaxDistanceTeleportationProvider _teleport;
        [SerializeField] private CesiumGeoreference _georef;
        [SerializeField] private GameObject _leftController;
        [SerializeField] private GameObject _rightController;
        [SerializeField] private GameObject _vignette;
        private InputAction _teleportAction;
        private InputAction _vignetteAction;
        private InputAction _resetPositionAction;
        private InputAction _snapTurnAction;
        private InputAction _mouseLookAction;

        public InputActionAsset InputAsset;
        public event Action<float> OnMoveSpeedChanged;
        public event Action<float> OnTurnSpeedChanged;
        public event Action<float> OnTurnAngleChanged;
        public event Action<bool>  OnVignetteChanged;

        [Header("Set to your terrain tiles layer")]
        [SerializeField] private LayerMask _groundMask = 0;

        [SerializeField] private float _walkHeightOffset = 3f;
        [SerializeField] private float _raycastMaxDist = 500f;
        //[SerializeField] private float _raycastMaxDistBuildingSelect =200f;
        [SerializeField] private float _searchRingStep = 2f; // meters between rings
        [SerializeField] private int _samplesPerRing = 24;

        public double3 FallbackGroundPosition = new double3(3741213.7858, 658435.3654, 5106351.1251);
        public Quaternion FallbackGroundRotation = Quaternion.Euler(new Vector3(-4.56f,19.9f,0f));
        public double3 FallbackGroundDirection = new double3(3740899.6478, 658550.12, 5106617.8534);
        public double3 FallbackFlyPosition = new double3(3741271.8927, 658446.675, 5106439.5336);
        public Quaternion FallbackFlyRotation = Quaternion.Euler(new Vector3(0f, 15f, 0f));
        public double3 FallbackFlyDirection = new double3(3740899.6478, 658550.12, 5106617.8534);
        public double3 TargetPosition = double3.zero;
        public Quaternion TargetRotation = Quaternion.identity;
        public double3 TargetDirection = double3.zero;
        public event Action<bool> OnFlyModeToggled;
        [Range(5f,1000f)] public float MaxFlyHeight = 1000f;
        
        private float _movementSpeed = 10f;
        /// <summary>
        /// Gets or sets the movement speed multiplier.
        /// Expects a value between 0.1 (slowest) and 1 (fastest).
        /// </summary>
        public float MovementSpeed
        {
            get => (_movementSpeed - 1f) / 19f;
            set
            {
                _movementSpeed = Mathf.Clamp(value, 0.1f, 1f) * 19f + 1f;
                ApplyMovementSpeeds();
                if (!_suppressPersistence)
                    _config.SaveUserSettingProperty<UserSettings, float>(u => u.MoveSpeed, MovementSpeed);
                OnMoveSpeedChanged?.Invoke(MovementSpeed);
            }
        }

        private bool _isDoubleMovementSpeed;
        public bool IsDoubleMovementSpeed
        {
            get => _isDoubleMovementSpeed;
            set
            {
                _isDoubleMovementSpeed = value;
                ApplyMovementSpeeds();
            }
        }

        private enum TurnMode { Continuous, Snap }
        private TurnMode _turnMode = TurnMode.Continuous;
        [SerializeField] private float _defaultSnapAngle = 30f;
        private float _lastNonZeroSnapAngle = 30f;

        private XROrigin _xrOrigin;
        private CesiumGlobeAnchor _cesiumGlobeAnchor;
        private bool _isFlying;
        private bool _isPositionInitialized;

        private float _pitch;
        private float _yaw;
        private float _pendingYawRotation;
        private InputAction _look;
        [SerializeField] float _sensitivity = 0.8f;
        [SerializeField] float _pitchClamp = 85f;

        private HmdPresenceMonitorService _hmdPresence;
        private ConfigurationService _config;
        private bool _suppressPersistence;
        
        private enum InputMode
        {
            Controller,
            Mouse
        }
        private InputMode _inputMode = InputMode.Mouse;

        private CesiumTilesLoadingProgressProvider _tilesLoadingProgressProvider;

        [SerializeField] private int _cityLayer;

        // Keyboard Teleport
        [SerializeField] private GameObject _teleportGo;
        private InputAction _keyboardTeleport;
        [SerializeField] private XRRayInteractor _teleportRay;
        [SerializeField] private float _kbTeleportYawSpeed = 0.12f;
        [SerializeField] private float _kbTeleportDistanceSpeed = 0.75f;
        [SerializeField] private float _kbTeleportMinDistance = 2f;
        [SerializeField] private float _kbTeleportMaxDistance = 40f;
        [SerializeField] private Vector3 _kbTeleportOffset = Vector3.left;
        private Transform _mouseAimAttach;
        private Transform _cachedAttach; // to restore after release
        private Transform _cachedRayOrigin;
        private bool _kbTeleportActive;
        private float _kbTeleportYaw;
        private float _kbTeleportDistance = 8f;

        [SerializeField] private Transform _defaultTeleportRayOrigin;


        void Awake()
        {
            _config = ServiceLocator.GetService<ConfigurationService>();
            //ToggleXRControllers();
            _tilesLoadingProgressProvider = FindFirstObjectByType<CesiumTilesLoadingProgressProvider>(FindObjectsInactive.Exclude);
            if (_tilesLoadingProgressProvider == null)
                ULog.Error("Missing CesiumTilesLoadingProgressProvider");
            _tilesLoadingProgressProvider.TilesLoaded += OnTilesLoaded;
            InputAction toggleFly = InputSystem.actions.FindAction("ToggleFly", true);
            //InputAction move= InputSystem.actions.FindAction("Move", true);
            toggleFly.performed += SetFlyModeToggle;
            toggleFly.Enable();
            _xrOrigin = GetComponent<XROrigin>();
            _xrOrigin.transform.position = new Vector3(0f, 200f, 0f); // into free space at start until tiles loaded
            _xrOrigin.transform.rotation = Quaternion.identity;
            _xrOrigin.Camera.transform.localPosition = Vector3.zero;
            _xrOrigin.Camera.transform.localRotation = Quaternion.identity;
            _cesiumGlobeAnchor = GetComponent<CesiumGlobeAnchor>();

            _look = InputSystem.actions.FindAction("Look", true);
            _look.Enable();
            _pitch = _xrOrigin.Camera.transform.eulerAngles.x;
            _yaw = _xrOrigin.Camera.transform.eulerAngles.y;

            // keyboard teleport
            _keyboardTeleport = InputSystem.actions.FindAction("KeyboardTeleport", true);
            _keyboardTeleport.Enable();
            _mouseAimAttach = new GameObject("MouseAimAttach").transform;
            _mouseAimAttach.SetParent(_xrOrigin.Camera.transform, false);
            _mouseAimAttach.localPosition = Vector3.zero;
            _mouseAimAttach.localRotation = Quaternion.identity;
            if (_teleportRay != null && _defaultTeleportRayOrigin == null)
                _defaultTeleportRayOrigin = _teleportRay.rayOriginTransform;
        }

        public async UniTask LoadUserSettingsAsync()
        {
            var userSettings = _config.LoadUserSettings<UserSettings>();
            _suppressPersistence = true; // prevent write-backs on load
            MovementSpeed = userSettings.MoveSpeed;
            SetTurnSpeed(userSettings.TurnSpeed);
            SetTurnAngle(userSettings.TurnAngle);
            SetVignette(userSettings.VignetteEnabled);
            _suppressPersistence = false;
            await UniTask.CompletedTask;
        }

        public void SetVignette(bool enable)
        {
            _vignette.SetActive(enable);
            if (!_suppressPersistence)
                _config.SaveUserSettingProperty<UserSettings, bool>(u => u.VignetteEnabled, enable);
            OnVignetteChanged?.Invoke(enable);
        }

        private void OnVignettePerformed(InputAction.CallbackContext ctx)
        {
            SetVignette(!_vignette.activeSelf);
        }

        private void OnTilesLoaded()
        {
            if (!_isPositionInitialized)
            {
                _georef.Initialize();
                _isPositionInitialized = true;
                ResetPosition();
            }
        }

        void Start()
        {
            _hmdPresence = ServiceLocator.GetService<HmdPresenceMonitorService>();
            if (_hmdPresence != null)
            {
                _inputMode = _hmdPresence.IsDeviceEnabled ? InputMode.Controller : InputMode.Mouse;
                ToggleXRControllers(_hmdPresence.CheckIfXRDeviceIsPresent());
                _hmdPresence.OnDeviceEnabled += OnDeviceChange;
            }
            else
            {
                _inputMode = InputMode.Mouse;
                ToggleXRControllers(false);
            }

            LayerManager.Instance.OnProjectChanged += OnProjectChanged;
            SetFlyMode(true); // start flying by default
            LoadUserSettingsAsync().Forget();
        }

        private void OnProjectChanged(Project p)
        {
            if (p != null)
                MaxFlyHeight = p?.MaximumFlyingHeight ?? 1000f;
            else
                MaxFlyHeight = 1000f;
        }

        void OnEnable()
        {
            _teleportAction = InputAsset.FindAction("XRI Left Locomotion/Teleport Mode", true); 
            //_teleportAction.performed += _ => Debug.Log("Teleport ACTION performed"); 
            _teleportAction.Enable();
            
            _vignetteAction = InputAsset.FindAction("Player/Vignette", true);
            _vignetteAction.performed += OnVignettePerformed;
            _vignetteAction.Enable();

            _resetPositionAction = InputAsset.FindAction("Player/Reset Position", true);
            _resetPositionAction.performed += OnResetPosition;
            _resetPositionAction.Enable();
            
            _snapTurnAction = InputAsset.FindAction("Player/Snap Turn", true);
            _snapTurnAction.performed += OnToggleSnapTurn;
            _snapTurnAction.Enable();
            
            _mouseLookAction = InputAsset.FindAction("Player/Mouse Look", true);
            _mouseLookAction.performed += OnMouseLookChanged;
            _mouseLookAction.Enable();

            _georef.Initialize();
            _georef.changed += OnGeoRefChanged;
        }

        void OnDisable()
        {
            if (_georef != null)
                _georef.changed -= OnGeoRefChanged;
            
            _tilesLoadingProgressProvider.TilesLoaded -= OnTilesLoaded;
            _vignetteAction.performed -= OnVignettePerformed;
            _resetPositionAction.performed -= OnResetPosition;
            _snapTurnAction.performed -= OnToggleSnapTurn;
            _mouseLookAction.performed -= OnMouseLookChanged;
            RestoreTeleportRayOrigin();
            _kbTeleportActive = false;
            if (_teleportGo != null) _teleportGo.SetActive(false);
            try
            {
                _hmdPresence.OnDeviceEnabled -= OnDeviceChange;
            }
            catch { }
        }

        private void OnDeviceChange(bool isEnabled)
        {
            _inputMode = isEnabled ? InputMode.Controller : InputMode.Mouse;
            //if (_isPositionInitialized)
            //    Cursor.visible = !isEnabled;
            ToggleXRControllers();
            if (_inputMode == InputMode.Controller)
            {
                _kbTeleportActive = false;
                _teleportGo.SetActive(false);
                RestoreTeleportRayOrigin(); 
                _mouseAimAttach.localPosition = Vector3.zero;
                _mouseAimAttach.localRotation = Quaternion.identity;
            }
            //else
            //{
            //    // reset FOV
            //    Camera.main.fieldOfView = 60f;
            //}
        }

        /// <summary>
        /// Mainly to get rid of the ray visuals when switching to mouse mode.
        /// </summary>
        private void ToggleXRControllers()
        {
            bool enable = _inputMode == InputMode.Controller;
            _leftController.SetActive(enable);
            _rightController.SetActive(enable);
            //if (_isPositionInitialized)
            //    Cursor.visible = !enable;
            if (enable)
            {
                _kbTeleportActive = false;
                _teleportGo.SetActive(false);
                RestoreTeleportRayOrigin();
            }
        }

        private void ToggleXRControllers(bool enable)
        {
            _leftController.SetActive(enable);
            _rightController.SetActive(enable);
            //if (_isPositionInitialized)
            //    Cursor.visible = !enable;
            _inputMode = enable ? InputMode.Controller : InputMode.Mouse;
            if (enable)
            {
                _kbTeleportActive = false;
                _teleportGo.SetActive(false);
                RestoreTeleportRayOrigin();
            }
        }

        private void OnMouseLookChanged(InputAction.CallbackContext ctx)
        {
            if (_look.enabled)
                _look.Disable();
            else
                _look.Enable();
        }

        private void OnGeoRefChanged()
        {
            // clamp height while flying
            if (_isFlying)
            {
                double rawHeight = _cesiumGlobeAnchor.longitudeLatitudeHeight.z;
                float clampedHeight = Mathf.Clamp((float)rawHeight, 1f, MaxFlyHeight);
                _cesiumGlobeAnchor.longitudeLatitudeHeight = new double3(
                    _cesiumGlobeAnchor.longitudeLatitudeHeight.x,
                    _cesiumGlobeAnchor.longitudeLatitudeHeight.y,
                    clampedHeight);
            }
        }

        private void RestoreTeleportRayOrigin()
        {
            if (_teleportRay == null) return;

            // Prefer an explicitly assigned origin; otherwise fall back to the first cached one we took
            Transform target = _defaultTeleportRayOrigin != null ? _defaultTeleportRayOrigin : _cachedRayOrigin;
            if (target != null)
                _teleportRay.rayOriginTransform = target;
        }

        void Update()
        {
            // Apply pending yaw rotation to XROrigin before movement providers calculate
            if (Mathf.Abs(_pendingYawRotation) > 0.001f)
            {
                _xrOrigin.RotateAroundCameraPosition(Vector3.up, _pendingYawRotation);
                _pendingYawRotation = 0f;
            }
        }

        void LateUpdate()
        {
            // Workaround for an issue where the Teleport action would not work.
            // Controller input action manager seems to have trouble enabling it by itself.
            // Possible fighting between two components.
            if (!_teleportAction.enabled)
            {
                //ULog.Info("was disabled");
                _teleportAction.Enable();
            }

            //if (InputSystem.GetDevice<UnityEngine.InputSystem.XR.XRController>(CommonUsages.LeftHand)
            //        ?.TryGetChildControl<ButtonControl>("triggerPressed")?.wasPressedThisFrame == true)
            //    Debug.Log("Teleport pressed");

            //if (_teleportAction.WasPressedThisFrame()) Debug.Log("Teleport ACTION fired update");

            if (_inputMode != InputMode.Mouse) return;

            // keyboard teleport
            bool ctrlHeld = _keyboardTeleport.IsPressed();
            bool ctrlUp = _keyboardTeleport.WasReleasedThisFrame();
            bool ctrlDown = _keyboardTeleport.WasPressedThisFrame();

            if (ctrlDown && _teleportRay != null)
            {
                _teleportGo.SetActive(true);
                _kbTeleportActive = true;
                _cachedRayOrigin = _teleportRay.rayOriginTransform;
                _teleportRay.rayOriginTransform = _mouseAimAttach;
                _mouseAimAttach.localPosition = _kbTeleportOffset;
                _kbTeleportYaw = 0f; // center forward
            }
            // While held: steer yaw (left/right) & distance (up/down)
            if (_kbTeleportActive && ctrlHeld)
            {
                Vector2 md = Mouse.current != null ? Mouse.current.delta.ReadValue() : Vector2.zero;

                _kbTeleportYaw += md.x * _kbTeleportYawSpeed; // left/right = yaw
                _kbTeleportDistance = Mathf.Clamp(
                    _kbTeleportDistance + md.y * _kbTeleportDistanceSpeed, // up/down = farther/closer
                    _kbTeleportMinDistance, _kbTeleportMaxDistance);

                Quaternion rot =
                    Quaternion.AngleAxis(_kbTeleportYaw, Vector3.up) *
                    Quaternion.AngleAxis(-5f, Vector3.right);
                _mouseAimAttach.localRotation = rot;

                // Drive distance using the interactor’s own property for the current line type
                switch (_teleportRay.lineType)
                {
                    case XRRayInteractor.LineType.StraightLine:
                        _teleportRay.maxRaycastDistance = _kbTeleportDistance;
                        break;
                    case XRRayInteractor.LineType.BezierCurve:
                        _teleportRay.endPointDistance = _kbTeleportDistance;
                        break;
                    default: // ProjectileCurve
                        _teleportRay.velocity = _kbTeleportDistance; // maps to throw distance
                        break;
                }
            }

            // release: teleport (if valid) and restore original attach
            if (_kbTeleportActive && ctrlUp)
            {
                _kbTeleportActive = false;

                if (_teleportRay.TryGetHitInfo(out var pos, out var normal, out _, out var valid) && valid)
                {
                    QueueTeleport(pos + Vector3.up * _walkHeightOffset, GetYawOnlyFacing(_xrOrigin.Camera.transform), false);
                }
                
                RestoreTeleportRayOrigin();
                _mouseAimAttach.localPosition = Vector3.zero; 
                _mouseAimAttach.localRotation = Quaternion.identity;
                _teleportGo.SetActive(false);
            }
           

            // Mouse look
            if (Mouse.current == null || !Mouse.current.rightButton.isPressed) return;
            Vector2 delta = _look.ReadValue<Vector2>() * (_sensitivity * Time.deltaTime);
            if (delta.sqrMagnitude < 1e-6f) return;
            // Accumulate yaw rotation for next frame
            _pendingYawRotation += delta.x;
            // Pitch: Apply to Camera only
            _pitch = Mathf.Clamp(_pitch - delta.y, -_pitchClamp, _pitchClamp);
            _xrOrigin.Camera.transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        private void ResetPosition()
        {
            TeleportTo(FallbackFlyPosition, FallbackFlyRotation, true);
        }

        private void OnResetPosition(InputAction.CallbackContext ctx)
        {
            ResetPosition();
        }

        private void SetTurnMode(TurnMode mode)
        {
            _turnMode = mode;
            bool useContinuous = mode == TurnMode.Continuous;
            _turn.enabled = useContinuous;
            _snapTurn.enabled = !useContinuous;
        }
        
        private void OnToggleSnapTurn(InputAction.CallbackContext ctx)
        {
            ToggleTurnMode();
        }

        public void ToggleTurnMode()
        {
            if (_turnMode == TurnMode.Continuous)
            {
                // going to Snap: ensure a sensible angle
                float targetAngle = (_snapTurn != null && _snapTurn.turnAmount > 0.001f)
                    ? _snapTurn.turnAmount
                    : (_lastNonZeroSnapAngle > 0.001f ? _lastNonZeroSnapAngle : _defaultSnapAngle);

                if (_snapTurn != null)
                    _snapTurn.turnAmount = targetAngle;

                _lastNonZeroSnapAngle = targetAngle;
                SetTurnMode(TurnMode.Snap);
            }
            else
            {
                // going to Continuous
                SetTurnMode(TurnMode.Continuous);
            }
        }

        public void SetTurnAngle(float angle)
        {
            angle = Mathf.Max(0f, angle);
            _snapTurn.turnAmount = angle;
            if (angle <= 0.001f)
            {
                // store last non-zero so keyboard toggle can restore it
                SetTurnMode(TurnMode.Continuous);
            }
            else
            {
                _lastNonZeroSnapAngle = angle;
                SetTurnMode(TurnMode.Snap);
            }
            if (!_suppressPersistence)
                _config.SaveUserSettingProperty<UserSettings, float>(u => u.TurnAngle, angle);
            OnTurnAngleChanged?.Invoke(angle);
        }

        public void SetTurnSpeed(float speed)
        {
            _turn.turnSpeed = 120f * speed;
            if (!_suppressPersistence)
                _config.SaveUserSettingProperty<UserSettings, float>(u => u.TurnSpeed, speed);
            OnTurnSpeedChanged?.Invoke(speed);
        }

        void OnDestroy()
        {
            InputAction toggleFly = InputSystem.actions.FindAction("ToggleFly", true);
            toggleFly.performed -= SetFlyModeToggle;
            toggleFly.Disable();
            _look.Disable();
        }

        private void SetFlyModeToggle(InputAction.CallbackContext ctx) 
        {
            SetFlyMode(!_isFlying);
        }

        /// <summary>
        /// Teleports the player to the specified position and rotation.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="ignoreDistanceLimit"></param>
        public void TeleportTo(double3 position, Quaternion rotation, bool ignoreDistanceLimit)
        {
            TargetPosition = position;
            TargetRotation = rotation;
            GotoLocation(ignoreDistanceLimit);
            //if (!_collab.IsGuidedModeEnabled) return;
            //if (!ServiceLocator.GetService<PermissionService>().IsModerator()) return;
            //SendCommandAsync("Teleport", x:position.x, y:position.y, z:position.z, rotation:rotation).Forget();
        }

        /// <summary>
        /// Teleports the player to the specified position and rotation.
        /// If the target position or rotation is not set, it will use the fallback position and rotation.
        /// </summary>
        private void GotoLocation(bool ignoreDistanceLimit)
        {
            var pos = !TargetPosition.Equals(double3.zero) ? TargetPosition : FallbackGroundPosition;
            ULog.Info($"RECV TargetPosition={TargetPosition} TargetRotation={TargetRotation} -> using pos={pos}");

            Quaternion rot;
            if (!TargetDirection.Equals(double3.zero))
            {
                double3 fwdECEF = TargetDirection - pos;
                double3 fwdU = _georef.TransformEarthCenteredEarthFixedDirectionToUnity(fwdECEF);
                Vector3 fwd = new Vector3((float)fwdU.x, (float)fwdU.y, (float)fwdU.z);
                rot = Quaternion.LookRotation(fwd != Vector3.zero ? fwd : Vector3.forward, Vector3.up);
            }
            else if (TargetRotation != Quaternion.identity)
            {
                rot = TargetRotation;
            }
            else
            {
                //double3 fwdECEF = FallbackGroundDirection - FallbackGroundPosition;
                //double3 fwdU = _georef.TransformEarthCenteredEarthFixedDirectionToUnity(fwdECEF);
                //Vector3 fwd = new Vector3((float)fwdU.x, (float)fwdU.y, (float)fwdU.z);
                //rot = Quaternion.LookRotation(fwd != Vector3.zero ? fwd : Vector3.forward, Vector3.up);
                rot = GetYawOnlyFacing(_xrOrigin.Camera.transform);
            }

            double3 geoPos = _georef.TransformEarthCenteredEarthFixedPositionToUnity(pos);
            Vector3 teleportPos = new Vector3((float)geoPos.x, (float)geoPos.y, (float)geoPos.z);

            QueueTeleport(teleportPos, rot, ignoreDistanceLimit);
            ULog.Info($"Teleported to {teleportPos} (rot {rot.eulerAngles.y:0}°)");
        }

        public void SetFlyMode(bool enable)
        {
            if (enable == _isFlying) return;
            _isFlying = enable;
            // first find ground spot that is safe to land on
            if (!_isFlying)
            {
                LandSafely();
            }
            // then switch movement modes and turn on gravity
            FlyingModeSwitch(_isFlying);
            
            OnFlyModeToggled?.Invoke(enable);
        }

        private void ApplyMovementSpeeds()
        {
            float doubleMult = IsDoubleMovementSpeed ? 2f : 1f;
            float baseSpeed = _movementSpeed * doubleMult;
            float effectiveSpeed = _isFlying ? baseSpeed * 10f : baseSpeed;

            if (_controllerMove != null) _controllerMove.moveSpeed = effectiveSpeed;
            if (_keyboardMove != null) _keyboardMove.moveSpeed = effectiveSpeed;
        }

        private void FlyingModeSwitch(bool flying)
        {
            _controllerFly.enabled = flying;
            _controllerMove.enableFly = flying;
            _keyboardFly.enabled = flying;
            _keyboardMove.enableFly = flying;
            _gravityProvider.useGravity = !flying;
            ApplyMovementSpeeds();
        }

        private void LandSafely()
        {
            var camPos = _xrOrigin.Camera.transform.position;
            if (TryFindGround(camPos, out var hit))
            {
                QueueTeleport(hit + Vector3.up * _walkHeightOffset, GetYawOnlyFacing(_xrOrigin.Camera.transform), true);
                ULog.Info("Successfully moved to landing position: " + hit);
            }
            else
            {
                TeleportTo(FallbackGroundPosition, FallbackGroundRotation, true);
                ULog.Info("No valid landing point found, using fallback location.");
            }
        }

        private void QueueTeleport(Vector3 destPos, Quaternion destRot, bool ignoreDistanceLimit)
        {
            TeleportRequest req = new()
            {
                destinationPosition = destPos,
                destinationRotation = destRot,
                matchOrientation = MatchOrientation.TargetUpAndForward, //MatchOrientation.WorldSpaceUp,
                requestTime = Time.time
            };
            //_teleport.QueueTeleportRequest(req);
            _teleport.QueueTeleportRequest(req, ignoreDistanceLimit);

            // command logic moved into teleport wrapper
            //var ecefPos = _georef.TransformUnityPositionToEarthCenteredEarthFixed(new double3(destPos));
            //if (_collab != null)
            //{
            //    if (!_collab.IsGuidedModeEnabled) return;
            //}
            //if (!ServiceLocator.GetService<PermissionService>().IsModerator()) return;
            //SendCommandAsync("Teleport", x: ecefPos.x, y: ecefPos.y, z: ecefPos.z, rotation: destRot).Forget();
        }

        private bool TryFindGround(Vector3 start, out Vector3 point)
        {
            if (RaycastDown(start, out point))
            {
                ULog.Info("Could walk directly under player.");
                return true;
            }

            for (float r = _searchRingStep; r <= _searchRingStep * 4; r += _searchRingStep)
            {
                float stepAngle = 360f / _samplesPerRing;
                for (int i = 0; i < _samplesPerRing; i++)
                {
                    float rad = Mathf.Deg2Rad * (i * stepAngle);
                    Vector3 offset = new(Mathf.Cos(rad) * r, 0, Mathf.Sin(rad) * r);
                    if (RaycastDown(start + offset, out point))
                    {
                        ULog.Info("Could not walk under player, but found ground at position: " + point);
                        return true;
                    }
                }
            }
            ULog.Info("Finished searching for ground but found no valid points.");

            point = Vector3.zero;
            return false;
        }

        private bool RaycastDown(Vector3 origin, out Vector3 hitPoint)
        {
            // added Vector3.down to origin to ensure the ray doesn't hit the player itself
            if (Physics.Raycast(origin + Vector3.down, Vector3.down, out var hit,
                    _raycastMaxDist, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                int layerBit = 1 << hit.collider.gameObject.layer;
                bool hitIsGround = (_groundMask.value & layerBit) != 0;

                if (hitIsGround)
                {
                    hitPoint = hit.point;
                    ULog.Info($"Hit ground object: {hit.collider.transform.parent.gameObject.name}");
                    return true;
                }
            }

            hitPoint = default;
            return false;       
        }

        private Quaternion GetYawOnlyFacing(Transform reference)
        {
            Vector3 fwd = reference.forward;
            fwd.y = 0f;
            if (fwd.sqrMagnitude < 1e-6f) fwd = _xrOrigin.transform.forward; // fallback
            return Quaternion.LookRotation(fwd.normalized, Vector3.up);
        }

        //private async UniTask SendCommandAsync(string type,
        //    bool value = false,
        //    double x = 0, double y = 0, double z = 0,
        //    string projectId = null,
        //    string variant = null,
        //    bool enabled = false,
        //    Quaternion rotation = default,
        //    int year = 0, int month = 0, int day = 0, int hour = 0, int minute = 0
        //)
        //{
        //    var json = JsonUtility.ToJson(new CommandPayload
        //    {
        //        Type = type,
        //        Value = value,
        //        X = x,
        //        Y = y,
        //        Z = z,
        //        ProjectId = projectId,
        //        Variant = variant,
        //        Enabled = enabled,
        //        Rotation = rotation,
        //        Year = year,
        //        Month = month,
        //        Day = day,
        //        Hour = hour,
        //        Minute = minute
        //    });

        //    try
        //    {
        //        var vivoxHandler = ServiceLocator.GetService<UGSService>().GetModule<VivoxHandler>();
        //        vivoxHandler.SendCommandMessageAsync(json, _collab.CommandChannelName).Forget();
        //    }
        //    catch (Exception ex)
        //    {
        //        ULog.Warning($"Failed to send command '{type}': {ex.Message}");
        //    }
        //    await UniTask.CompletedTask;
        //}
    }
}