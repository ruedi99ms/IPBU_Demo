using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Numena.SC;

namespace Numena.SC.UI
{
    [RequireComponent(typeof(Button))]
    public class SC_ToggleButton : MonoBehaviour
    {
        [SerializeField] Image toggleLight;
        public Color toggleOffColor = Color.gray;
        public Color toggleOnColor = Color.green;
        [SerializeField] bool _on = false;
        public bool on { get => _on; set => SetOn(value); }

        Button _targetButton;
        public Button targetButton { get { if (_targetButton == null) _targetButton = GetComponent<Button>(); return _targetButton; } }

        public Action<SC_ToggleButton> onToggle;

        private void OnEnable()
        {
            targetButton.onClick.AddListener(OnClick);
            toggleLight.CrossFadeColor(_on ? toggleOnColor : toggleOffColor, 0.1f, true, true);
        }

        private void OnDisable()
        {
            targetButton.onClick.RemoveListener(OnClick);
        }

        void OnClick()
        {
            //SetOn(!_on);
            onToggle?.Invoke(this);
        }

        void SetOn(bool value)
        {
            if (_on == value)
                return;
            _on = value;
            toggleLight.CrossFadeColor(_on ? toggleOnColor : toggleOffColor, 0.1f, true, true);
        }
    }
}
