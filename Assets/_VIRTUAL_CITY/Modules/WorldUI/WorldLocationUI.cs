using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Numena.SC;

namespace Numena.SC.UI
{
    public class WorldLocationUI : MonoBehaviour
    {
        [SerializeField] RectTransform locationContainer;
        [SerializeField] SC_LocationUIItem itemPrefab;
        [SerializeField, Tooltip("Distance from player in meter")] float canvasDistance;
        [SerializeField] CanvasGroup canvasGroup;

        private List<SC_LocationUIItem> items = new List<SC_LocationUIItem>();
        private bool _togglePressed;
        private bool _isVisible;
        private Transform _canvasRoot;
        private Coroutine _transition;

        private Transform _rigTransform;
        private Transform _camTransform;

        public bool IsActive { get; set; }

        private void Start()
        {
            List<SC_LocationInfo> locations = SC_LocationManager.GetAllLocations();
            foreach (SC_LocationInfo loc in locations)
            {
                SC_LocationUIItem item = Instantiate(itemPrefab, locationContainer);
                item.targetLocation = loc;
                items.Add(item);
                item.onSelect += OnItemSelected;
            }

            _rigTransform = SC_VRRig.rigTransform;
            _camTransform = SC_VRRig.mainCamera.transform;
            _canvasRoot = GetComponentInParent<Canvas>().transform;

            _isVisible = false;
            _transition = StartCoroutine(SetVisible(false));
        }

        private void Update()
        {
            InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primaryButton, out bool pressed);
            if (!pressed)
                InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.secondaryButton, out pressed);
            if (!pressed)
                InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primaryButton, out pressed);
            if (!pressed)
                InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.secondaryButton, out pressed);

            if (!_togglePressed)
            {
                if (pressed)
                {
                    _togglePressed = true;
                    Toggle();
                }
            }
            else if (!pressed)
            {
                _togglePressed = false;
            }

            if (_isVisible && Vector3.Distance(transform.position, _rigTransform.position) > 5f)
            {
                Toggle();
            }
        }

        private void OnDestroy()
        {
            foreach (SC_LocationUIItem item in items)
            {
                item.onSelect -= OnItemSelected;
            }
        }

        void OnItemSelected(SC_LocationUIItem item)
        {
            Toggle();
        }

        public void Toggle()
        {
            if (_transition != null)
                StopCoroutine(_transition);

            _isVisible = !_isVisible;
            _transition = StartCoroutine(SetVisible(_isVisible));
        }

        private IEnumerator SetVisible(bool show)
        {
            canvasGroup.interactable = show;
            canvasGroup.blocksRaycasts = show;
            if (show)
            {
                _canvasRoot.position = _camTransform.position + _camTransform.forward * canvasDistance;
                _canvasRoot.rotation = Quaternion.LookRotation(_canvasRoot.position - _camTransform.position);
            }

            float target = show ? 1f : 0f;
            float alpha = canvasGroup.alpha;
            while (alpha != target)
            {
                alpha = Mathf.MoveTowards(alpha, target, Time.deltaTime);
                canvasGroup.alpha = alpha;
                yield return new WaitForEndOfFrame();
            }
            _transition = null;
        }

        SC_LocationUIItem GetItem(SC_Location loc)
        {
            foreach (SC_LocationUIItem item in items)
            {
                if (item.targetLocation == loc)
                    return item;
            }
            return null;
        }
    }
}