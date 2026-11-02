using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Numena.SC
{
    /// <summary>
    /// Canbe used to activate and deactivate boundaries to restrict the players movement. 
    /// </summary>
    [DefaultExecutionOrder(-100)]//initialize before boundaries
    public class SC_BoundaryManager : MonoBehaviour
    {
        static SC_BoundaryManager main;

        SC_Boundary _activeBoundary = null;
        [SerializeField] SC_Boundary startBoundary = null;

        public static Action<SC_Boundary, RaycastHit> onBoundaryHit;

        public static bool active { get { return main.enabled; } set { main.enabled = value; } }

        private void Awake()
        {
            main = this;
        }

        private void OnEnable()
        {
            if (_activeBoundary != null)
                _activeBoundary.gameObject.SetActive(true);
        }

        private void Start()
        {
            if (startBoundary != null)
                SetActiveBoundary(startBoundary);
        }

        private void OnDisable()
        {
            if (_activeBoundary != null)
                _activeBoundary.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            main = null;
        }

        public static void SetActiveBoundary(SC_Boundary boundary)
        {
            if (main._activeBoundary != null)
                main._activeBoundary.gameObject.SetActive(false);
            main._activeBoundary = boundary;
            if (main._activeBoundary != null)
                main._activeBoundary.gameObject.SetActive(true);
        }
    }
}
