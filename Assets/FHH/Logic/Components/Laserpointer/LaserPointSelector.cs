using Cysharp.Threading.Tasks;
using FHH.Input;
using Foxbyte.Core;
using FHH.Logic.Components.HmdPresenceMonitor;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;

namespace FHH.Logic.Components.Laserpointer
{
    public class LaserPointSelector : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private NearFarInteractor _nearFarInteractor;
        [SerializeField]
        private NearFarInteractor _nearFarInteractorToDeactivate;

        [SerializeField] private Transform _indicatorTransform;

        [Header("Desktop Raycast Settings")] [SerializeField]
        private Camera _raycastCamera;

        [SerializeField] private LayerMask _raycastLayerMask = Physics.DefaultRaycastLayers;

        [SerializeField] private float _raycastMaxDistance = 100f;

        [Header("Input Actions")] [Tooltip("XR controller 'LaserPointer' or similar action.")] [SerializeField]
        private InputActionReference _laserPointerAction;

        [Tooltip("Optional: left mouse click or similar action.")] [SerializeField]
        private InputActionReference _mouseClickAction;

        [Header("Indicator Settings")] [SerializeField]
        private float _indicatorDurationSeconds = 2f;

        [Tooltip("If true, laser is disabled again after a point was selected.")] [SerializeField]
        private bool _disableLaserAfterSelection = true;

        /// <summary>
        /// Fired when a selection attempt during laserpointer mode
        /// does not find a valid hit point.
        /// </summary>
        public event Action LaserPointerSelectionFailed;

        /// <summary>
        /// Fired when the laserpointer "waiting for click" mode changes.
        /// True when entering selection mode, false when leaving.
        /// </summary>
        public event Action<bool> LaserPointerWaitingChanged;

        /// <summary>
        /// Fired once when the user clicks during laserpointer mode
        /// and a valid 3D hit point could be determined.
        /// </summary>
        public event Action<Vector3> PointSelected;

        private bool _isWaitingForPoint;

        private CancellationTokenSource _indicatorCts;

        private bool _hasPendingSelection;

        private enum InputMode
        {
            Controller,
            Mouse
        }

        private InputMode _inputMode = InputMode.Mouse;
        private int _laserModeEnteredFrame = -1;

        private HmdPresenceMonitorService _hmdPresence;

        void Awake()
        {
            UpdateInputMode();

            if (_nearFarInteractor == null)
            {
                _nearFarInteractor = GetComponentInChildren<NearFarInteractor>(true);
            }
            if (_indicatorTransform != null)
            {
                _indicatorTransform.gameObject.SetActive(false);
                var visual = _indicatorTransform.GetComponent<LaserPointerIndicatorVisual>();
                if (visual != null && visual.enabled)
                {
                    if (_raycastCamera == null)
                    {
                        _raycastCamera = Camera.main;
                    }
                    visual.Cam = _raycastCamera;
                }
            }
            SetLaserActive(false, false);
            if (_raycastCamera == null)
            {
                _raycastCamera = Camera.main;
            }
        }

        void OnEnable()
        {
            //SubscribeInputActions();
        }

        void OnDisable()
        {
            UnsubscribeInputActions();

            CancelIndicator();

            if (_isWaitingForPoint || _hasPendingSelection)
            {
                CancelLaserSelection();
            }

            SetLaserActive(false, false);
        }

        private void OnDestroy()
        {
            CancelIndicator();
        }

        private void UpdateInputMode()
        {
            if (_hmdPresence == null)
            {
                try
                {
                    _hmdPresence = ServiceLocator.GetService<HmdPresenceMonitorService>();
                }
                catch
                {
                    _hmdPresence = null;
                }
            }

            if (_hmdPresence != null)
            {
                _inputMode = _hmdPresence.IsDeviceEnabled ? InputMode.Controller : InputMode.Mouse;
            }
            else
            {
                _inputMode = InputMode.Mouse;
            }
        }

        private void SubscribeInputActions()
        {
            if (_laserPointerAction != null && _laserPointerAction.action != null)
            {
                _laserPointerAction.action.performed += OnConfirmInputPerformed;
            }

            if (_mouseClickAction != null && _mouseClickAction.action != null)
            {
                _mouseClickAction.action.performed += OnConfirmInputPerformed;
            }
        }

        private void UnsubscribeInputActions()
        {
            if (_laserPointerAction != null && _laserPointerAction.action != null)
            {
                _laserPointerAction.action.performed -= OnConfirmInputPerformed;
            }

            if (_mouseClickAction != null && _mouseClickAction.action != null)
            {
                _mouseClickAction.action.performed -= OnConfirmInputPerformed;
            }
        }

        private void OnConfirmInputPerformed(InputAction.CallbackContext context)
        {
            if (!isActiveAndEnabled)
            {
                return;
            }
            UpdateInputMode();
            if (_laserModeEnteredFrame == Time.frameCount)
            {
                // This is the same press that toggled the UI button to enter laser mode; ignore it
                Debug.LogError("Same frame");
                return;
            }
            if (!_isWaitingForPoint)
            {
                return;
            }

            if (_hasPendingSelection)
            {
                return;
            }

            Vector3 hitPoint;
            bool hasPoint = false;

            if (_inputMode == InputMode.Controller)
            {
                // XR controller mode: treat this as controller confirm regardless
                // of which of the subscribed actions fired.
                Debug.Log("LaserPointSelector: Confirm input received in controller mode.");

                if (_nearFarInteractor == null)
                {
                    Debug.LogWarning("LaserPointSelector: No NearFarInteractor assigned, cannot get XR hit point.");
                    LaserPointerSelectionFailed?.Invoke();
                    ExitLaserPointerMode(_disableLaserAfterSelection);
                    return;
                }

                hasPoint = TryGetHitPointFromInteractor(out hitPoint);
                if (hasPoint)
                {
                    Debug.Log("LaserPointSelector: XR hit point found at " + hitPoint);
                }
                else
                {
                    Debug.Log("LaserPointSelector: No valid XR hit point found.");
                }
            }
            else
            {
                // Mouse / desktop mode: only react to the mouse action
                if (_mouseClickAction == null || _mouseClickAction.action == null || context.action != _mouseClickAction.action)
                {
                    return;
                }

                Debug.Log("LaserPointSelector: Confirm input received from mouse action (mouse mode).");
                hasPoint = TryGetHitPointFromMouseRay(out hitPoint);
            }

            if (!hasPoint)
            {
                Debug.Log("LaserPointSelector: No valid hit point found.");
                LaserPointerSelectionFailed?.Invoke();
                ExitLaserPointerMode(_disableLaserAfterSelection);
                return;
            }
            CommitSelectionDeferredAsync(hitPoint).Forget();
        }

        private void ExitLaserPointerMode(bool disableLaser)
        {
            if (!_isWaitingForPoint)
            {
                return;
            }

            _isWaitingForPoint = false;
            _hasPendingSelection = false;

            LaserPointerWaitingChanged?.Invoke(false);

            if (disableLaser)
            {
                SetLaserActive(false, false);
            }
        }

        //XR hit provider
        private bool TryGetHitPointFromInteractor(out Vector3 hitPoint)
        {
            hitPoint = Vector3.zero;

            if (_nearFarInteractor == null)
            {
                return false;
            }

            ICurveInteractionDataProvider curveProvider = _nearFarInteractor;
            EndPointType endPointType = curveProvider.TryGetCurveEndPoint(
                out hitPoint,
                false,
                false
            );

            switch (endPointType)
            {
                case EndPointType.ValidCastHit:
                case EndPointType.EmptyCastHit:
                case EndPointType.UI:
                    return true;
                default:
                    return false;
            }
        }

        //mouse-based raycast hit provider
        private bool TryGetHitPointFromMouseRay(out Vector3 hitPoint)
        {
            hitPoint = Vector3.zero;

            Camera cameraToUse = _raycastCamera != null ? _raycastCamera : Camera.main;
            if (cameraToUse == null)
            {
                Debug.LogWarning("LaserPointSelector: No camera available for mouse raycast.");
                return false;
            }

            if (Mouse.current == null)
            {
                Debug.LogWarning("LaserPointSelector: No mouse device available for raycast.");
                return false;
            }

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = cameraToUse.ScreenPointToRay(mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, _raycastMaxDistance, _raycastLayerMask))
            {
                hitPoint = hit.point;
                return true;
            }

            return false;
        }

        private void SetLaserActive(bool active, bool nearFarActive)
        {
            if (_nearFarInteractor == null)
            {
                Debug.LogWarning("LaserPointSelector: Cannot set laser active, NearFarInteractor is not assigned.");
                return;
            }

            _nearFarInteractor.gameObject.SetActive(nearFarActive);
            if (_inputMode == InputMode.Controller && _nearFarInteractorToDeactivate != null)
            {
                _nearFarInteractorToDeactivate.gameObject.SetActive(!nearFarActive);
            }

            if (active)
            {
                _laserPointerAction.action.Enable();
                SubscribeInputActions();

            }
            else
            {
                _laserPointerAction.action.Disable();
                UnsubscribeInputActions();
            }
        }

        private void CancelIndicator()
        {
            if (_indicatorCts != null)
            {
                _indicatorCts.Cancel();
                _indicatorCts.Dispose();
                _indicatorCts = null;
            }
            if (_indicatorTransform != null)
            {
                _indicatorTransform.gameObject.SetActive(false);
            }
        }

        private async UniTask RunIndicatorAsync(Vector3 position, CancellationToken ct)
        {
            if (_indicatorTransform == null)
            {
                return;
            }
            if (_indicatorTransform.GetComponent<LaserPointerIndicatorVisual>() == null)
            {
                _indicatorTransform.position = position;
            }
            _indicatorTransform.gameObject.SetActive(true);
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_indicatorDurationSeconds), cancellationToken: ct);
            }
            catch (OperationCanceledException)
            {
            }
            if (!ct.IsCancellationRequested && _indicatorTransform != null)
            {
                _indicatorTransform.gameObject.SetActive(false);
            }
        }

        // deferred selection commit to avoid UI-toggle race
        private async UniTask CommitSelectionDeferredAsync(Vector3 hitPoint)
        {
            _hasPendingSelection = true;

            // Allow UI toggle / CancelLaserSelection to run in the same frame
            // and cancel this pending selection before it commits
            await UniTask.WaitForEndOfFrame();

            if (!_isWaitingForPoint || !_hasPendingSelection)
            {
                // Selection was canceled or mode exited in the meantime
                _hasPendingSelection = false;
                return;
            }
            ShowIndicatorAt(hitPoint);
            PointSelected?.Invoke(hitPoint);

            Debug.Log("LaserPointSelector: Point selected at " + hitPoint);

            _hasPendingSelection = false;
            ExitLaserPointerMode(_disableLaserAfterSelection);
        }

        // Public API

        /// <summary>
        /// Enters laserpointer mode: enables the laser and waits
        /// for a confirm input (XR UI press or mouse click).
        /// When the input is received and a valid hit exists,
        /// the <see cref="PointSelected"/> event is fired.
        /// </summary>
        public void ActivateLaserAndAwaitPoint()
        {
            UpdateInputMode();

            if (_isWaitingForPoint)
            {
                // Already in laserpointer mode; ignore re-entrance
                return;
            }
            ActivateAsync().Forget(ex => ULog.Error($"Error activating laser pointer {ex}"));
        }

        private async UniTask ActivateAsync()
        {
            await UniTask.WaitForEndOfFrame();
            // Ensure any existing indicator is stopped before starting a new selection
            CancelIndicator();
            await UniTask.DelayFrame(1);
            _hasPendingSelection = false;
            _isWaitingForPoint = true;
            LaserPointerWaitingChanged?.Invoke(true);
            _laserModeEnteredFrame = Time.frameCount;
            bool enableInteractor = _inputMode == InputMode.Controller;
            await UniTask.WaitForEndOfFrame();
            SetLaserActive(true, enableInteractor);
        }

        /// <summary>
        /// Shows the indicator at the given position for a few seconds,
        /// without doing any raycast.
        /// </summary>
        public void ShowIndicatorAt(Vector3 position)
        {
            if (_indicatorTransform == null)
            {
                return;
            }

            CancelIndicator();

            _indicatorCts = new CancellationTokenSource();
            CancellationToken ct = _indicatorCts.Token;

            var visual = _indicatorTransform.GetComponent<LaserPointerIndicatorVisual>();
            if (visual != null)
            {
                visual.SetTargetPosition(position);
            }
            else
            {
                _indicatorTransform.position = position;
            }

            RunIndicatorAsync(position, ct).Forget();
        }

        /// <summary>
        /// Cancels the current laser selection mode, if active.
        /// No <see cref="PointSelected"/> event is fired.
        /// The laser is disabled after cancellation.
        /// </summary>
        public void CancelLaserSelection()
        {
            if (!_isWaitingForPoint && !_hasPendingSelection)
            {
                return;
            }
            CancelIndicator();
            _hasPendingSelection = false;
            if (_isWaitingForPoint)
            {
                _isWaitingForPoint = false;
                LaserPointerWaitingChanged?.Invoke(false);
            }
            SetLaserActive(false, false);
        }
    }
}
