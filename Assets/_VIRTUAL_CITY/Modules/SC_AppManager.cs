using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Numena.SC
{
    /// <summary>
    /// Placeholder. Manages app lifecycle.
    /// </summary>
    public class SC_AppManager : MonoBehaviour
    {
        static SC_AppManager main;

        private void Awake()
        {
            if (main != null)
                Debug.LogError(nameof(SC_AppManager) + " is a singleton. Please make sure there is only one instance in the scene.");
            main = this;
        }

        /// <summary>
        /// can be called from unityevents. For prototyping only.
        /// </summary>
        public void ExitApp()
        {
            Application.Quit();
        }
    }
}
