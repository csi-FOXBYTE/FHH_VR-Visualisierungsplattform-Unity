using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;


namespace FHH.Input
{
    public class BlockXRWhenOverUI : XRBaseTargetFilter
    {
        [SerializeField] private EventSystem _eventSystem;   // NEW: instance ref (drag your EventSystem or leave null)
        private XRUIInputModule _xrUi;                       // NEW: cached XR UI module instance

        public override void Link(IXRInteractor interactor)
        {
            // NEW: resolve references when the interactor links
            if (_eventSystem == null) _eventSystem = EventSystem.current;
            if (_eventSystem != null) _xrUi = _eventSystem.GetComponent<XRUIInputModule>();
        }

        public override void Unlink(IXRInteractor interactor) { }

        public override void Process(IXRInteractor interactor, List<IXRInteractable> targets, List<IXRInteractable> results)
        {
            results.Clear();

            // NEW: block when any XR pointer is currently over UI (UITK or UGUI)
            if (_xrUi != null && _xrUi.IsPointerOverGameObject(-1))
                return; // results stays empty → interactor has no valid targets this frame

            // Optional mouse/touch fallback if you also use them:
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            // Otherwise pass through the original targets
            if (targets.Count > 0) results.AddRange(targets);
        }
    }
}