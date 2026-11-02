using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using Numena.SC;

namespace Numena.SC.UI
{
    [RequireComponent(typeof(InputSystemUIInputModule))]
    public class RayLineRenderer : MonoBehaviour
    {
        [SerializeField] Transform cursorTransform;
        [SerializeField] Transform sourceTransform;
        [SerializeField] LineRenderer line;

        private InputSystemUIInputModule _inputModule;
        private UnityEngine.InputSystem.InputDevice _inputDevice;
        private UnityEngine.InputSystem.Utilities.InternedString _strRightHand = new UnityEngine.InputSystem.Utilities.InternedString("RightHand");

        private Renderer _cursorRenderer;
        private bool _isReady = false;

        private void Awake()
        {
            InputSystem.onDeviceChange += OnDeviceChanged;
            foreach (InputDevice device in InputSystem.devices)
            {
                if (device.enabled == false)
                    continue;
                OnDeviceChanged(device, InputDeviceChange.Added);
            }
        }

        private void Start()
        {
            _cursorRenderer = cursorTransform.GetComponent<Renderer>();
            _inputModule = GetComponent<InputSystemUIInputModule>();
        }

        private void OnDeviceChanged(UnityEngine.InputSystem.InputDevice device, InputDeviceChange change)
        {
            if (!device.usages.Contains(_strRightHand))
                return;

            if (change == InputDeviceChange.UsageChanged
                || change == InputDeviceChange.ConfigurationChanged)
                return;

            bool validState = (change == InputDeviceChange.Added)
                || (change == InputDeviceChange.Reconnected)
                || (change == InputDeviceChange.Enabled);

            if (_isReady != validState)
            {
                _inputDevice = validState ? device : null;
                _isReady = validState;
            }
        }

        private void LateUpdate()
        {
            if (!_isReady)
                return;

            RaycastResult hit = _inputModule.GetLastRaycastResult(_inputDevice.deviceId);
            line.enabled = hit.isValid;
            _cursorRenderer.enabled = hit.isValid;
            if (!hit.isValid)
                return;

            line.SetPosition(0, Vector3.Lerp(sourceTransform.position, hit.worldPosition, .5f));
            line.SetPosition(1, Vector3.Lerp(sourceTransform.position, hit.worldPosition, .95f));
            cursorTransform.position = hit.worldPosition;
        }
    }
}