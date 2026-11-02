using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Numena.SC
{
    /// <summary>
    /// Allows switching between different VR locomotion types.
    /// </summary>
    [DefaultExecutionOrder(-100)]//independent managers
    public class SC_LocomotionManager : MonoBehaviour
    {
        static SC_LocomotionManager main;

        [SerializeField] GameObject _currentLocomotion;
        public static GameObject currentLocomotion => main._currentLocomotion;
        public static Action onLocomotionChanged;

        [SerializeField] GameObject _teleportLocomotion, _flyLocomotion;

        public static GameObject teleportLocomotion => main._teleportLocomotion;
        public static GameObject flyLocomotion => main._flyLocomotion;

        private void Awake()
        {
            if (main != null)
                Debug.LogError(nameof(SC_LocomotionManager) + " is a singleton. Please make sure there is only one instance in the scene.");
            main = this;
        }

        private void OnDestroy()
        {
            main = null;
        }

        public static void SetLocomotion(GameObject locomotion)
        {
            if (main == null)
            {
                Debug.LogError("SetLocomotion requires a LocomotionManager in the scene.");
                return;
            }

            if (locomotion == main._currentLocomotion)
                return;
            if (main._currentLocomotion != null)
                main._currentLocomotion.SetActive(false);

            main._currentLocomotion = locomotion;

            if (main._currentLocomotion != null)
                main._currentLocomotion.SetActive(true);

            onLocomotionChanged?.Invoke();
        }
    }
}
