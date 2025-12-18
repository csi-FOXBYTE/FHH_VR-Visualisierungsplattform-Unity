using System.Collections;
using FHH.Logic.Cesium;
using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.InputSystem;

namespace FHH.Logic.VR
{
    /// <summary>
    /// Purpose: Place/rotate a world-space UITK panel relative to the XR camera.
    /// </summary>
    [DisallowMultipleComponent]
    public class WorldSpaceUiFollower : MonoBehaviour
    {
        [SerializeField] private XROrigin _xrOrigin;
        [SerializeField] private Camera _xrCamera;

        [Header("Placement")] [SerializeField] private float _distance = 1.6f; // meters in front of HMD
        [SerializeField] private float _verticalOffset = 0f; // meters above HMD
        [SerializeField] private bool _yawOnly = true; // ignore head pitch/roll for placement/rotation
        [SerializeField] private bool _billboard = true; // face the camera
        [SerializeField] private bool _continuousFollow = true; // follow every frame; off = call SnapNow() yourself
        [SerializeField] private float _smoothTime = 0.12f; // smoothing for motion; 0 = instant
        [SerializeField] private InputActionAsset _inputAsset;
        [SerializeField] private CesiumTilesLoadingProgressProvider _tilesLoadingProgressProvider;
        private InputAction _snapAction;
        private Vector3 _vel;
        private Vector3 _followOffset;

        private void OnEnable()
        {
            _snapAction = _inputAsset.FindAction("Snap UI");
            _snapAction.Enable();
            _snapAction.performed += OnSnapAction;
            _tilesLoadingProgressProvider.TilesLoaded += SnapNow;
        }

        void Start()
        {
            StartCoroutine(SnapLater());
            Application.onBeforeRender += BeforeRenderFollow;
        }

        private void BeforeRenderFollow()
        {
            if (_continuousFollow) UpdateTransform(false);
        }

        private void OnDisable()
        {
            _snapAction.performed -= OnSnapAction;
            _tilesLoadingProgressProvider.TilesLoaded -= SnapNow;
            Application.onBeforeRender -= BeforeRenderFollow;
        }

        private void OnDestroy()
        {
            Application.onBeforeRender -= BeforeRenderFollow;
        }

        private void OnSnapAction(InputAction.CallbackContext context)
        {
            SnapNow();
        }

        private IEnumerator SnapLater()
        {
            yield return new WaitForSeconds(2f);
            SnapNow();
        }

        void Reset()
        {
            _xrOrigin = FindFirstObjectByType<XROrigin>();
            if (_xrOrigin != null) _xrCamera = _xrOrigin.Camera;
            if (_xrCamera == null) _xrCamera = Camera.main;
        }

        public void SnapNow()
        {
            UpdateTransform(true);
        }

        //void LateUpdate()
        //{
        //    if (_continuousFollow) UpdateTransform(Time.deltaTime, false);
        //}

        private void UpdateTransform(bool isSnap)
        {
            if (isSnap)
            {
                Transform camT = _xrCamera.transform;
                Vector3 forward = _yawOnly
                    ? Vector3.ProjectOnPlane(camT.forward, Vector3.up).normalized
                    : camT.forward;
                Vector3 targetPos = camT.position + forward * _distance + Vector3.up * _verticalOffset;
                transform.position = targetPos;
                if (_billboard)
                {
                    if (_yawOnly)
                        transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
                    else
                        transform.rotation = Quaternion.LookRotation((transform.position - camT.position).normalized, Vector3.up);
                }
                _followOffset = transform.position - GetRigPosition();
                _vel = Vector3.zero;
            }
            else
            {
                //Vector3 targetPos = GetRigPosition() + _followOffset;
                Vector3 targetPos = _xrOrigin.transform.position + _followOffset;
                
                if (_smoothTime <= 0f)
                {
                    transform.position = targetPos;
                }
                else
                {
                    transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _vel, _smoothTime, Mathf.Infinity, Time.deltaTime);
                }
            }
        }
        
        private Vector3 GetRigPosition()
        {
            if (_xrOrigin != null) return _xrOrigin.transform.position;
            if (_xrCamera != null) return _xrCamera.transform.position;
            return transform.position;
        }
        
    }
}