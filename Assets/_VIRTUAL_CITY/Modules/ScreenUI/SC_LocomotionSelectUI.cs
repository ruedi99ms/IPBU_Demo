using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Numena.SC;

namespace Numena.SC.UI
{
    public class SC_LocomotionSelectUI : MonoBehaviour
    {
        [SerializeField] SC_ToggleButton teleportBtn, flyBtn;

        private void OnEnable()
        {
            SC_LocomotionManager.onLocomotionChanged += OnLocomotionChanged;
            teleportBtn.onToggle += OnTeleportClicked;
            flyBtn.onToggle += OnFlyClicked;
            UpdateButtons();
        }

        private void OnDisable()
        {
            SC_LocomotionManager.onLocomotionChanged -= OnLocomotionChanged;
            teleportBtn.onToggle -= OnTeleportClicked;
            flyBtn.onToggle -= OnFlyClicked;
        }

        void OnTeleportClicked(SC_ToggleButton btn)
        {
            SC_LocomotionManager.SetLocomotion(SC_LocomotionManager.teleportLocomotion);
        }

        void OnFlyClicked(SC_ToggleButton btn)
        {
            SC_LocomotionManager.SetLocomotion(SC_LocomotionManager.flyLocomotion);
        }

        void OnLocomotionChanged()
        {
            UpdateButtons();
        }

        void UpdateButtons()
        {
            teleportBtn.on = SC_LocomotionManager.currentLocomotion == SC_LocomotionManager.teleportLocomotion;
            flyBtn.on = SC_LocomotionManager.currentLocomotion == SC_LocomotionManager.flyLocomotion;
        }
    }
}
